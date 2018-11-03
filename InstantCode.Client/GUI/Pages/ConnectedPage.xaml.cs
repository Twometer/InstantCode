using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows.Controls;
using EnvDTE;
using InstantCode.Client.GUI.Model;
using InstantCode.Client.Network;
using InstantCode.Protocol.Packets;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace InstantCode.Client.GUI.Pages
{
    /// <summary>
    /// Interaction logic for ConnectedPage.xaml
    /// </summary>
    public partial class ConnectedPage : UserControl
    {
        private ServerListPage parent;
        private IPageSwitcher pageSwitcher;

        private ServerEntry currentServer;

        public ConnectedPage(ServerListPage parent, IPageSwitcher pageSwitcher, ServerEntry currentServer)
        {
            this.parent = parent;
            this.pageSwitcher = pageSwitcher;
            this.currentServer = currentServer;
            InitializeComponent();
            Header.Content = $"Connected to {currentServer.Name} ({currentServer.Ip})";
            LoadUsers();
        }

        private async void LoadUsers()
        {
            var userList = await InstantCodeClient.Instance.WaitForReplyAsync<P0AUserList>();
            foreach (var user in userList.UserList)
                OnlineUsersBox.Items.Add(user);
        }

        private async void OpenSessionButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var progressDialog = new ProgressDialog("Creating session...", () => { }, true);
            progressDialog.Show();

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var dte = (DTE)Package.GetGlobalService(typeof(DTE));
            var solution = dte?.Solution;

            if (solution == null || !solution.IsOpen)
            {
                new ErrorDialog("Please open a VisualStudio solution before creating a session").ShowModal();
                progressDialog.Close();
                return;
            }

            if (!solution.Saved)
            {
                new ErrorDialog("Please save the current solution before creating a session").ShowModal();
                progressDialog.Close();
                return;
            }

            var participants = OnlineUsersBox.SelectedItems.Cast<string>().ToArray();

            InstantCodeClient.Instance.SendPacket(new P02NewSession(solution.Projects.Item(1).Name, participants));
            var statePacket = await InstantCodeClient.Instance.WaitForReplyAsync<P01State>();
            if (statePacket.ReasonCode != ReasonCode.Ok)
            {
                new ErrorDialog($"Failed to create session: {statePacket.ReasonCode}").ShowModal();
                progressDialog.Close();
                return;
            }
            InstantCodeClient.Instance.CurrentSessionId = statePacket.Payload;

            progressDialog.StatusMessage = "Compressing project...";
            var solutionFolder = new FileInfo(solution.FileName).Directory;
            var zipFile = new FileInfo(Path.Combine(solutionFolder.Parent.FullName, $"{solutionFolder.Name}{statePacket.Payload:X}.ic.zip"));
            ZipSolution(solutionFolder.FullName, zipFile.FullName);

            progressDialog.StatusMessage = "Transmitting project...";
            progressDialog.IsIntermediate = false;
            await TransmitAsync(zipFile, p => progressDialog.Value = p);
            progressDialog.Close();
        }

        private static void ZipSolution(string solutionFolderPath, string zipFilePath)
        {
            using (var zipToOpen = new FileStream(zipFilePath, FileMode.Create))
            using (var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
            {
                foreach (var file in Directory.GetFiles(solutionFolderPath, "*", SearchOption.AllDirectories))
                {
                    var entryName = file.Substring(solutionFolderPath.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                    var entry = archive.CreateEntry(entryName);
                    entry.LastWriteTime = File.GetLastWriteTime(file);
                    using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                    using (var stream = entry.Open())
                        fs.CopyTo(stream, 81920);
                }
            }
        }

        private async Task TransmitAsync(FileInfo zipFile, Action<int> progressChanged)
        {
            var file = zipFile.OpenRead();
            InstantCodeClient.Instance.SendPacket(new P04OpenStream((int)zipFile.Length));
            await InstantCodeClient.Instance.WaitForReplyAsync<P01State>();

            var transmitted = 0;
            var buffer = new byte[8192];
            while (true)
            {
                var read = await file.ReadAsync(buffer, 0, buffer.Length);
                if (read < 0)
                    break;
                transmitted += read;

                var sendBuffer = new byte[read];
                Array.Copy(buffer, 0, sendBuffer, 0, sendBuffer.Length);

                InstantCodeClient.Instance.SendPacket(new P05StreamData(sendBuffer));

                progressChanged((int)(transmitted / (double)zipFile.Length * 100d));
            }
            InstantCodeClient.Instance.SendPacket(new P06CloseStream());
            zipFile.Delete();
        }

        private void DisconnectButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            InstantCodeClient.Instance.Disconnect();
            pageSwitcher.SwitchPage(parent);
        }
    }
}
