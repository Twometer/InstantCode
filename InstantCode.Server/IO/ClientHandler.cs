using System.Net.Sockets;
using InstantCode.Protocol;
using InstantCode.Protocol.IO;
using InstantCode.Server.Crypto;

namespace InstantCode.Server.IO
{
    public class ClientHandler
    {
        private readonly TcpClient tcpClient;
        private readonly FixedDataStream dataStream;

        public ClientHandler(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
            dataStream = new FixedDataStream(tcpClient.GetStream());
        }

        public void StartReading()
        {
            while (tcpClient.Connected)
            {
                
            }

            ClientManager.RemoveClient(this);
        }

        // Packet layout: | IV | Encrypted Packet length | Packet Id | Packet content | 
        //                                               |<------AES ENCRYPTED------->|
        public void SendPacket(IPacket packet)
        {
            var innerBuffer = new PacketBuffer();
            innerBuffer.WriteInt(packet.Id);
            innerBuffer.WriteArray(PacketSerializer.Serialize(packet));

            var encryptedPacket = PacketCrypto.Encrypt(innerBuffer.ToArray(), out var iv);
            var outerBuffer = new PacketBuffer();
            outerBuffer.WriteArray(iv);
            outerBuffer.WriteArray(encryptedPacket);

            var finalPacket = outerBuffer.ToArray();
            dataStream.Write(finalPacket, 0, finalPacket.Length);
        }
    }
}
