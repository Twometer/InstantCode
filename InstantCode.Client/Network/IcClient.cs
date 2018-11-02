using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using InstantCode.Protocol;
using InstantCode.Protocol.Crypto;
using InstantCode.Protocol.IO;
using InstantCode.Protocol.Packets;

namespace InstantCode.Client.Network
{
    public class IcClient
    {
        private TcpClient tcpClient;
        private FixedDataStream dataStream;

        private PacketHandler packetHandler;

        private static readonly IPacket[] RegisteredPackets =
        {
            new P01State()
        };

        public void Connect(string server, int port, string password)
        {
            tcpClient = new TcpClient(server, port);
            dataStream = new FixedDataStream(tcpClient.GetStream());
            packetHandler = new PacketHandler();
            CredentialStore.Store(password);
        }

        public void SendPacket(IPacket packet)
        {
            var serializedPacket = PacketSerializer.Serialize(packet, CredentialStore.KeyHash);
            dataStream.Write(serializedPacket, 0, serializedPacket.Length);
        }

        public void StartReading()
        {
            // TODO: This is not how you should use the Tasks API
            Task.Run(() =>
            {
                while (tcpClient.Connected)
                {
                    var packetBuf = PacketSerializer.Deserialize(dataStream, CredentialStore.KeyHash);
                    HandlePacket(packetBuf);
                }
            });
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
                break;
            }
        }

        public void Disconnect()
        {
            tcpClient.Close();
        }

    }
}
