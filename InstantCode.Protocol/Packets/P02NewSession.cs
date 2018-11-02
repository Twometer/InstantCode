using InstantCode.Protocol.Handler;
using InstantCode.Protocol.IO;

namespace InstantCode.Protocol.Packets
{
    public class P02NewSession : IPacket
    {
        public int Id => 0x02;

        public string ProjectName;
        public string[] Participants;

        public P02NewSession()
        {

        }

        public P02NewSession(string projectName, string[] participants)
        {
            ProjectName = projectName;
            Participants = participants;
        }

        public void Read(PacketBuffer buffer)
        {
            ProjectName = buffer.ReadString();

            var participantsLen = buffer.ReadInt();
            Participants = new string[participantsLen];
            for (var i = 0; i < participantsLen; i++)
                Participants[i] = buffer.ReadString();
        }

        public void Write(PacketBuffer buffer)
        {
            buffer.WriteString(ProjectName);
            buffer.WriteInt(Participants.Length);
            foreach(var participant in Participants)
                buffer.WriteString(participant);
        }

        public void Handle(INetHandler netHandler)
        {
            netHandler.HandleP02NewSession(this);
        }
    }
}
