using InstantCode.Protocol.Handler;
using InstantCode.Protocol.IO;

namespace InstantCode.Protocol.Packets
{
    public class P05StreamData : IPacket
    {
        public int Id => 0x05;

        public byte[] Data { get; set; }

        public P05StreamData()
        {

        }

        public P05StreamData(byte[] data)
        {
            Data = data;
        }

        public void Read(PacketBuffer buffer)
        {
            Data = buffer.ReadArray();
        }

        public void Write(PacketBuffer buffer)
        {
            buffer.WriteArray(Data);
        }

        public void Handle(INetHandler netHandler)
        {
            netHandler.HandleP05StreamData(this);
        }
    }
}
