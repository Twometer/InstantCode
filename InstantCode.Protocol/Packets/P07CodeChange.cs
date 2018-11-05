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
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public string Data { get; set; }

        public P07CodeChange()
        {

        }

        public P07CodeChange(int sessionId, string sender, string file, int startIndex, int endIndex, string data)
        {
            SessionId = sessionId;
            Sender = sender;
            File = file;
            StartIndex = startIndex;
            EndIndex = endIndex;
            Data = data;
        }

        public void Read(PacketBuffer buffer)
        {
            SessionId = buffer.ReadInt();
            Sender = buffer.ReadString();
            File = buffer.ReadString();
            StartIndex = buffer.ReadInt();
            EndIndex = buffer.ReadInt();
            Data = buffer.ReadString();
        }

        public void Write(PacketBuffer buffer)
        {
            buffer.WriteInt(SessionId);
            buffer.WriteString(Sender);
            buffer.WriteString(File);
            buffer.WriteInt(StartIndex);
            buffer.WriteInt(EndIndex);
            buffer.WriteString(Data);
        }

        public void Handle(INetHandler netHandler)
        {
            netHandler.HandleP07CodeChange(this);
        }
    }
}
