using InstantCode.Protocol.Handler;
using InstantCode.Protocol.IO;

namespace InstantCode.Protocol.Packets
{
    public class P07CodeChange : IPacket
    {
        public int Id => 0x07;

        public int SessionId { get; set; }
        public string Sender { get; set; }
        public string File { get; set; }
        public int Index { get; set; }
        public string Char { get; set; }

        public P07CodeChange()
        {

        }

        public P07CodeChange(int sessionId, string sender, string file, int index, string @char)
        {
            SessionId = sessionId;
            Sender = sender;
            File = file;
            Index = index;
            Char = @char;
        }

        public void Read(PacketBuffer buffer)
        {
            SessionId = buffer.ReadInt();
            Sender = buffer.ReadString();
            File = buffer.ReadString();
            Index = buffer.ReadInt();
            Char = buffer.ReadString();
        }

        public void Write(PacketBuffer buffer)
        {
            buffer.WriteInt(SessionId);
            buffer.WriteString(Sender);
            buffer.WriteString(File);
            buffer.WriteInt(Index);
            buffer.WriteString(Char);
        }

        public void Handle(INetHandler netHandler)
        {
            netHandler.HandleP07CodeChange(this);
        }
    }
}
