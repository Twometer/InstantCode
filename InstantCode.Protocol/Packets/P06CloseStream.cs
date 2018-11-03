using InstantCode.Protocol.Handler;
using InstantCode.Protocol.IO;

namespace InstantCode.Protocol.Packets
{
    public class P06CloseStream : IPacket
    {
        public int Id => 0x06;

        public void Read(PacketBuffer buffer)
        {
        }

        public void Write(PacketBuffer buffer)
        {
        }

        public void Handle(INetHandler netHandler)
        {
            netHandler.HandleP06CloseStream(this);
        }
    }
}
