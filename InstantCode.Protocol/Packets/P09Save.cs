using System;
using System.Collections.Generic;
using System.Text;
using InstantCode.Protocol.Handler;
using InstantCode.Protocol.IO;

namespace InstantCode.Protocol.Packets
{
    public class P09Save : IPacket
    {
        public int Id => 0x09;

        public int SessionId;

        public P09Save()
        {

        }

        public P09Save(int sessionId)
        {
            SessionId = sessionId;
        }

        public void Read(PacketBuffer buffer)
        {
            SessionId = buffer.ReadInt();
        }

        public void Write(PacketBuffer buffer)
        {
            buffer.WriteInt(SessionId);
        }

        public void Handle(INetHandler netHandler)
        {
            netHandler.HandleP09Save(this);
        }
    }
}
