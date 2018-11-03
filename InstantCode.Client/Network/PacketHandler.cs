using System;
using System.IO;
using InstantCode.Client.GUI;
using InstantCode.Protocol.Handler;
using InstantCode.Protocol.Packets;

namespace InstantCode.Client.Network
{
    public class PacketHandler : INetHandler
    {
        private static readonly string FolderPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "InstantCode");

        private ProgressDialog progressDialog;
        private Stream currentStream;

        private int streamRead;
        private int streamLength;

        public void HandleP00Login(P00Login p00Login)
        {
            
        }

        public void HandleP01State(P01State p01State)
        {
            
        }

        public void HandleP02NewSession(P02NewSession p02NewSession)
        {
            progressDialog = new ProgressDialog($"You have been added to session '{p02NewSession.ProjectName}'. Receiving data...", () => {}, true);
            InstantCodeClient.Instance.CurrentSessionName = p02NewSession.ProjectName;
        }

        public void HandleP03CloseSession(P03CloseSession p03CloseSession)
        {
            
        }

        public void HandleP04OpenStream(P04OpenStream p04OpenStream)
        {
            progressDialog.IsIntermediate = false;
            streamLength = p04OpenStream.DataLength;
            currentStream = File.OpenWrite(Path.Combine(FolderPath,
                $"{InstantCodeClient.Instance.CurrentSessionName}{InstantCodeClient.Instance.CurrentSessionId:X}.zip"));
        }

        public void HandleP05StreamData(P05StreamData p05StreamData)
        {
            currentStream.Write(p05StreamData.Data, 0, p05StreamData.Data.Length);
            streamRead += p05StreamData.Data.Length;
            progressDialog.Value = (int) (streamRead / (double) streamLength * 100);
        }

        public void HandleP06CloseStream(P06CloseStream p06CloseStream)
        {
            currentStream.Close();
            progressDialog.Close();
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

        public void HandleP0AUserList(P0AUserList p0AUserList)
        {
            
        }
    }
}
