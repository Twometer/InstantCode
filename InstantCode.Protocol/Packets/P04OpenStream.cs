using InstantCode.Protocol.Handler;
using InstantCode.Protocol.IO;

namespace InstantCode.Protocol.Packets
{
    public class P04OpenStream : IPacket
    {
        public int Id => 0x04;

        public int DataLength { get; set; }

        public P04OpenStream()
        {

        }

        public P04OpenStream(int dataLength)
        {
            DataLength = dataLength;
        }

        public void Read(PacketBuffer buffer)
        {
            DataLength = buffer.ReadInt();
        }

        public void Write(PacketBuffer buffer)
        {
            buffer.WriteInt(DataLength);
        }

        public void Handle(INetHandler netHandler)
        {
            netHandler.HandleP04OpenStream(this);
        }
    }
}
