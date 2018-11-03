using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using InstantCode.Protocol;
using InstantCode.Protocol.Crypto;
using InstantCode.Protocol.IO;
using InstantCode.Protocol.Packets;
using InstantCode.Client.Utils;

namespace InstantCode.Client.Network
{
    public class InstantCodeClient
    {
        public static InstantCodeClient Instance = new InstantCodeClient();

        private TcpClient tcpClient;
        private FixedDataStream dataStream;

        private PacketHandler packetHandler;

        private PacketAwaitItem awaitItem = new PacketAwaitItem();

        private static readonly IPacket[] RegisteredPackets =
        {
            new P01State()
        };

        public async Task<T> WaitForReplyAsync<T>() where T : IPacket
        {
            awaitItem.Reset();
            awaitItem.PacketId = Activator.CreateInstance<T>().Id;
            await awaitItem.WaitHandle.WaitOneAsync();
            return (T)awaitItem.Packet;
        }

        public async Task ConnectAsync(string server, int port, string password)
        {
            tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(server, port);
            dataStream = new FixedDataStream(tcpClient.GetStream());
            packetHandler = new PacketHandler();
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
                var packetBuf = await PacketSerializer.Deserialize(dataStream, CredentialStore.KeyHash);
                HandlePacket(packetBuf);
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

                if (pack.Id == awaitItem.PacketId)
                {
                    awaitItem.Packet = pack;
                    awaitItem.WaitHandle.Set();
                }
                break;
            }
        }

        public void Disconnect()
        {
            tcpClient.Close();
        }

    }
}
