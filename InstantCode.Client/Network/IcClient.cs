using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using InstantCode.Protocol;
using InstantCode.Protocol.Crypto;
using InstantCode.Protocol.IO;

namespace InstantCode.Client.Network
{
    public class IcClient
    {
        private TcpClient tcpClient;
        private FixedDataStream dataStream;

        public void Connect(string server, int port, string password)
        {
            tcpClient = new TcpClient(server, port);
            dataStream = new FixedDataStream(tcpClient.GetStream());
            CredentialStore.Store(password);
        }

        public void SendPacket(IPacket packet)
        {
            var serializedPacket = PacketSerializer.Serialize(packet, CredentialStore.KeyHash);
            dataStream.Write(serializedPacket, 0, serializedPacket.Length);
        }

        public void Disconnect()
        {
            tcpClient.Close();
        }

    }
}
