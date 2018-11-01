using InstantCode.Protocol.IO;

namespace InstantCode.Protocol
{
    public interface IPacket
    {
        int Id { get; }

        void Read(PacketBuffer buffer);

        void Write(PacketBuffer buffer);

    }
}
