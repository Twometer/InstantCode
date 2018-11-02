using System;
using System.Net.Sockets;
using InstantCode.Protocol;
using InstantCode.Protocol.Crypto;
using InstantCode.Protocol.Handler;
using InstantCode.Protocol.IO;
using InstantCode.Protocol.Packets;
using InstantCode.Server.Config;
using InstantCode.Server.Utility;

namespace InstantCode.Server.IO
{
    public class ClientHandler
    {
        private const string Tag = "ClientHandler";

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
            Log.I(Tag, "Client " + Util.GetIp(tcpClient) + " connected");
            while (tcpClient.Connected)
            {
                try
                {
                    var iv = ReadRawArray();
                    var content = ReadRawArray();
                    HandlePacket(new PacketBuffer(PacketCrypto.Decrypt(content, CredentialStore.KeyHash, iv)));
                }
                catch (Exception e)
                {
                    Log.I(Tag, "Client " + Util.GetIp(tcpClient) + " lost connection: " + e);
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
            var serialized = PacketSerializer.Serialize(packet, CredentialStore.KeyHash);
            dataStream.Write(serialized, 0, serialized.Length);
        }
    }
}
