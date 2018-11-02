using System;
using System.Collections.Generic;
using System.Text;
using InstantCode.Protocol.Handler;
using InstantCode.Protocol.IO;

namespace InstantCode.Protocol.Packets
{
    public class P03CloseSession : IPacket
    {
        public int Id => 0x03;

        public int SessionId { get; set; }

        public P03CloseSession()
        {

        }

        public P03CloseSession(int sessionId)
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
            netHandler.HandleP03CloseSession(this);
        }
    }
}
