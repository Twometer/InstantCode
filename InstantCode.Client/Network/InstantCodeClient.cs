using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using InstantCode.Client.GUI;
using InstantCode.Client.GUI.Pages;
using InstantCode.Client.Model;
using InstantCode.Protocol;
using InstantCode.Protocol.Crypto;
using InstantCode.Protocol.IO;
using InstantCode.Protocol.Packets;
using InstantCode.Client.Utils;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace InstantCode.Client.Network
{
    public class InstantCodeClient
    {
        public static InstantCodeClient Instance = new InstantCodeClient();

        public string CurrentUsername { get; set; }
        public Session CurrentSession { get; set; }
        public Action DisconnectHandler { get; set; }

        private TcpClient tcpClient;
        private FixedDataStream dataStream;
        private PacketHandler packetHandler;

        private readonly PacketAwaitHandler awaitHandler = new PacketAwaitHandler();

        private bool forcedClose;

        private static readonly IPacket[] RegisteredPackets =
        {
            new P01State(),
            new P02NewSession(),
            new P03CloseSession(),
            new P04OpenStream(),
            new P05StreamData(),
            new P06CloseStream(),
            new P07CodeChange(),
            new P08CursorPosition(),
            new P09Save(),
            new P0AUserList()
        };

        public async Task<T> WaitForReplyAsync<T>() where T : IPacket
        {
            awaitHandler.Reset();
            awaitHandler.PacketId = Activator.CreateInstance<T>().Id;
            await awaitHandler.WaitHandle.WaitOneAsync();
            return (T)awaitHandler.Packet;
        }

        public async Task ConnectAsync(IPageSwitcher pageSwitcher, string server, int port, string password)
        {
            tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(server, port);
            dataStream = new FixedDataStream(tcpClient.GetStream());
            packetHandler = new PacketHandler(pageSwitcher);
            CredentialStore.Store(password);
            StartReadingAsync();
        }

        public InstantCodeClient SendPacket(IPacket packet)
        {
            var serializedPacket = PacketSerializer.Serialize(packet, CredentialStore.KeyHash);
            dataStream.Write(serializedPacket, 0, serializedPacket.Length);
            return this;
        }

        private async Task StartReadingAsync()
        {
            while (tcpClient.Connected)
            {
                try
                {
                    HandlePacket(await PacketSerializer.Deserialize(dataStream, CredentialStore.KeyHash));
                }
                catch(Exception e)
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    DisconnectHandler?.Invoke();
                    if (forcedClose)
                    {
                        forcedClose = false;
                        return;
                    }
                    new ErrorDialog("Connection to the server has been lost: " + e.Message).ShowModal();
                    break;
                }
            }
        }

        private void HandlePacket(PacketBuffer outerBuffer)
        {
            var packetId = outerBuffer.ReadInt();
            var packetContent = new PacketBuffer(outerBuffer.ReadArray());
            foreach (var pack in RegisteredPackets)
            {
                if (pack.Id != packetId) continue;
                pack.Read(packetContent);
                pack.Handle(packetHandler);

                if (pack.Id == awaitHandler.PacketId)
                {
                    awaitHandler.Packet = pack;
                    awaitHandler.WaitHandle.Set();
                }
                break;
            }
        }

        public void Disconnect()
        {
            tcpClient.Close();
            tcpClient.Dispose();
            CurrentSession = null;
            forcedClose = true;
        }

    }
}
