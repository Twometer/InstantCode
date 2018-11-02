using System;
using System.IO;
using System.Text;

namespace InstantCode.Protocol.IO
{
    public class PacketBuffer
    {
        private readonly MemoryStream memoryStream;

        public PacketBuffer()
        {
            memoryStream = new MemoryStream();
        }

        public PacketBuffer(byte[] data)
        {
            memoryStream = new MemoryStream(data);
        }

        public void WriteInt(int val)
        {
            WriteRaw(BitConverter.GetBytes(val));
        }

        public void WriteArray(byte[] data)
        {
            WriteInt(data.Length);
            WriteRaw(data);
        }

        public void WriteString(string data)
        {
            WriteArray(Encoding.UTF8.GetBytes(data));
        }

        public void WriteBool(bool b)
        {
            memoryStream.WriteByte((byte) (b ? 1 : 0));
        }

        public int ReadInt()
        {
            return BitConverter.ToInt32(ReadRaw(4), 0);
        }

        public byte[] ReadArray()
        {
            return ReadRaw(ReadInt());
        }

        public string ReadString()
        {
            return Encoding.UTF8.GetString(ReadArray());
        }

        public bool ReadBool()
        {
            return memoryStream.ReadByte() == 1;
        }

        private byte[] ReadRaw(int len)
        {
            var buf = new byte[len];
            memoryStream.Read(buf, 0, buf.Length);
            return buf;
        }

        private void WriteRaw(byte[] data)
        {
            memoryStream.Write(data, 0, data.Length);
        }

        public byte[] ToArray()
        {
            return memoryStream.ToArray();
        }
    }
}
