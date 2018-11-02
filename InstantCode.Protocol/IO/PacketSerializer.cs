using System;
using System.Collections.Generic;
using System.Text;
using InstantCode.Protocol.Crypto;

namespace InstantCode.Protocol.IO
{
    public class PacketSerializer
    {
        public static byte[] Serialize(IPacket packet, byte[] cryptoKey)
        {
            var innerBuffer = new PacketBuffer();
            innerBuffer.WriteInt(packet.Id);
            innerBuffer.WriteArray(SerializeContents(packet));

            var encryptedPacket = PacketCrypto.Encrypt(innerBuffer.ToArray(), cryptoKey, out var iv);
            var outerBuffer = new PacketBuffer();
            outerBuffer.WriteArray(iv);
            outerBuffer.WriteArray(encryptedPacket);

            return outerBuffer.ToArray();
        }

        private static byte[] SerializeContents(IPacket packet)
        {
            var buffer = new PacketBuffer();
            packet.Write(buffer);
            return buffer.ToArray();
        }
    }
}
