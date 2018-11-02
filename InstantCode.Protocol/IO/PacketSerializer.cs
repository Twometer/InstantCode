using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using InstantCode.Protocol.Crypto;

namespace InstantCode.Protocol.IO
{
    public class PacketSerializer
    {
        public static async Task<PacketBuffer> Deserialize(FixedDataStream dataStream, byte[] cryptoKey)
        {
            var iv = await ReadRawArray(dataStream);
            var content = await ReadRawArray(dataStream);
            return new PacketBuffer(PacketCrypto.Decrypt(content, CredentialStore.KeyHash, iv));
        }

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

        private static async Task<byte[]> ReadRawArray(Stream dataStream)
        {
            var raw = new byte[4];
            await dataStream.ReadAsync(raw, 0, raw.Length);
            var len = BitConverter.ToInt32(raw, 0);
            var array = new byte[len];
            await dataStream.ReadAsync(array, 0, array.Length);
            return array;
        }
    }
}
