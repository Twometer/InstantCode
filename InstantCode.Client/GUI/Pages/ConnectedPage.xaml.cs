using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows.Controls;
using EnvDTE;
using InstantCode.Client.GUI.Model;
using InstantCode.Client.Network;
using InstantCode.Client.Utils;
using InstantCode.Protocol.Packets;
using Microsoft.VisualStudio.Shell;

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
            var progressDialog = new ProgressDialog("Creating session...", () => { });
            progressDialog.Show();

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var dte = (DTE) Package.GetGlobalService(typeof(DTE));
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

            progressDialog.StatusMessage = "Transmitting project...";
            var solutionFolder = new FileInfo(solution.FileName).Directory;
        }


        private void DisconnectButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            InstantCodeClient.Instance.Disconnect();
            pageSwitcher.SwitchPage(parent);
        }
    }
}
