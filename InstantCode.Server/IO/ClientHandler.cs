using System;
using System.Net.Sockets;
using InstantCode.Protocol;
using InstantCode.Protocol.Handler;
using InstantCode.Protocol.IO;
using InstantCode.Protocol.Packets;
using InstantCode.Server.Crypto;

namespace InstantCode.Server.IO
{
    public class ClientHandler
    {
        private readonly TcpClient tcpClient;
        private readonly INetHandler netHandler;
        private readonly FixedDataStream dataStream;
       
        private static readonly IPacket[] RegisteredPackets =
        {
            new P00Login(),
        };

        public ClientHandler(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
            netHandler = new PacketHandler(this);
            dataStream = new FixedDataStream(tcpClient.GetStream());
        }

        public void StartReading()
        {
            while (tcpClient.Connected)
            {
                try
                {
                    var iv = ReadRawArray();
                    var content = ReadRawArray();
                    HandlePacket(new PacketBuffer(PacketCrypto.Decrypt(content, iv)));
                }
                catch (Exception e)
                {
                    Console.WriteLine();
                    break;
                }
            }

            ClientManager.RemoveClient(this);
        }

        private void HandlePacket(PacketBuffer outerBuffer)
        {
            var packetId = outerBuffer.ReadInt();
            var packetContent = new PacketBuffer(outerBuffer.ReadArray());
            foreach (var pack in RegisteredPackets)
            {
                if (pack.Id != packetId) continue;
                pack.Read(packetContent);
                pack.Handle(netHandler);
                break;
            }
        }

        private byte[] ReadRawArray()
        {
            var raw = new byte[4];
            dataStream.Read(raw, 0, raw.Length);
            var len = BitConverter.ToInt32(raw);
            var array = new byte[len];
            dataStream.Read(array, 0, array.Length);
            return array;
        }

        // Packet layout: | IV | Packet Id | Packet content | 
        //                     |<-----AES ENCRYPTED ARR---->|
        //
        // all arrays are length-prefixed
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
