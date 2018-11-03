using InstantCode.Protocol.Handler;
using InstantCode.Protocol.IO;

namespace InstantCode.Protocol.Packets
{
    public class P08CursorPosition : IPacket
    {
        public int Id => 0x08;

        public int SessionId { get; set; }
        public string Sender { get; set; }
        public string File { get; set; }
        public int NewIndex { get; set; }

        public P08CursorPosition()
        {

        }

        public P08CursorPosition(int sessionId, string sender, string file, int newIndex)
        {
            SessionId = sessionId;
            Sender = sender;
            File = file;
            NewIndex = newIndex;
        }

        public void Read(PacketBuffer buffer)
        {
            SessionId = buffer.ReadInt();
            Sender = buffer.ReadString();
            File = buffer.ReadString();
            NewIndex = buffer.ReadInt();
        }

        public void Write(PacketBuffer buffer)
        {
            buffer.WriteInt(SessionId);
            buffer.WriteString(Sender);
            buffer.WriteString(File);
            buffer.WriteInt(NewIndex);
        }

        public void Handle(INetHandler netHandler)
        {
            netHandler.HandleP08CursorPosition(this);
        }
    }
}
