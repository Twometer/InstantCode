using InstantCode.Protocol.Handler;
using InstantCode.Protocol.IO;

namespace InstantCode.Protocol.Packets
{
    public class P01State : IPacket
    {
        public int Id => 0x01;

        public ReasonCode ReasonCode { get; set; }
        public int Payload { get; set; }

        public P01State()
        {

        }

        public P01State(ReasonCode reasonCode)
        {
            ReasonCode = reasonCode;
        }

        public P01State(ReasonCode reasonCode, int payload)
        {
            ReasonCode = reasonCode;
            Payload = payload;
        }

        public void Read(PacketBuffer buffer)
        {
            ReasonCode = (ReasonCode)buffer.ReadInt();
            Payload = buffer.ReadInt();
        }

        public void Write(PacketBuffer buffer)
        {
            buffer.WriteInt((int)ReasonCode);
            buffer.WriteInt(Payload);
        }

        public void Handle(INetHandler netHandler)
        {
            netHandler.HandleP01State(this);
        }
    }

    public enum ReasonCode
    {
        Ok,
        UsernameTaken,
        NoPermission,
        SessionJoined
    }
}
