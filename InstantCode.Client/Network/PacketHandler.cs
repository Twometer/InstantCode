using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using EnvDTE;
using InstantCode.Client.GUI;
using InstantCode.Client.GUI.Pages;
using InstantCode.Protocol.Handler;
using InstantCode.Protocol.Packets;
using Microsoft.VisualStudio.Shell;

namespace InstantCode.Client.Network
{
    public class PacketHandler : INetHandler
    {
        private static readonly string FolderPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "InstantCode");

        private IPageSwitcher pageSwitcher;

        private ProgressDialog progressDialog;
        private Stream currentStream;

        private int streamRead;
        private int streamLength;

        public PacketHandler(IPageSwitcher pageSwitcher)
        {
            this.pageSwitcher = pageSwitcher;
        }

        public void HandleP00Login(P00Login p00Login)
        {
            
        }

        public void HandleP01State(P01State p01State)
        {
            if (p01State.ReasonCode == ReasonCode.SessionJoined)
                InstantCodeClient.Instance.CurrentSessionId = p01State.Payload;
        }

        public async void HandleP02NewSession(P02NewSession p02NewSession)
        {
            InstantCodeClient.Instance.CurrentSessionName = p02NewSession.ProjectName;
            InstantCodeClient.Instance.CurrentSessionParticipants = p02NewSession.Participants;
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            progressDialog = new ProgressDialog($"Joining session '{p02NewSession.ProjectName}'...", () => { }, true);
            progressDialog.Show();
        }

        public void HandleP03CloseSession(P03CloseSession p03CloseSession)
        {
            
        }

        public async void HandleP04OpenStream(P04OpenStream p04OpenStream)
        {
            streamLength = p04OpenStream.DataLength;
            currentStream = File.OpenWrite(Path.Combine(FolderPath,
                $"{InstantCodeClient.Instance.CurrentSessionName}{InstantCodeClient.Instance.CurrentSessionId:X}.zip"));
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            progressDialog.IsIntermediate = false;
        }

        public async void HandleP05StreamData(P05StreamData p05StreamData)
        {
            currentStream.Write(p05StreamData.Data, 0, p05StreamData.Data.Length);
            streamRead += p05StreamData.Data.Length;
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            progressDialog.Value = (int) (streamRead / (double) streamLength * 100);
        }

        public async void HandleP06CloseStream(P06CloseStream p06CloseStream)
        {
            currentStream.Close();
            var zipFile = Path.Combine(FolderPath,
                $"{InstantCodeClient.Instance.CurrentSessionName}{InstantCodeClient.Instance.CurrentSessionId:X}.zip");
            var targetFolder = Path.Combine(FolderPath,
                $"{InstantCodeClient.Instance.CurrentSessionName}{InstantCodeClient.Instance.CurrentSessionId:X}");

            if (!Directory.Exists(targetFolder))
                Directory.CreateDirectory(targetFolder);

            ZipFile.ExtractToDirectory(zipFile,targetFolder);
            var solutionFile = Directory.GetFiles(targetFolder).Single(f => f.EndsWith(".sln"));

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            progressDialog.Close();

            var dte = (DTE)Package.GetGlobalService(typeof(DTE));
            dte.Solution.Open(solutionFile);

            pageSwitcher.SwitchPage(new SessionPage());
        }

        public void HandleP07CodeChange(P07CodeChange p07CodeChange)
        {
            
        }

        public void HandleP08CursorPosition(P08CursorPosition p08CursorPosition)
        {
            
        }

        public void HandleP09Save(P09Save p09Save)
        {
            
        }

        public async void HandleP0AUserList(P0AUserList p0AUserList)
        {
            if (pageSwitcher.GetCurrentPage() is ConnectedPage page)
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                page.OnlineUsersBox.Items.Clear();
                foreach (var itm in p0AUserList.UserList)
                    page.OnlineUsersBox.Items.Add(itm);
            }
        }
    }
}
