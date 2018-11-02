﻿using System;
using System.Collections.Generic;
using System.Text;
using InstantCode.Protocol.Handler;
using InstantCode.Protocol.IO;

namespace InstantCode.Protocol.Packets
{
    public class P01State : IPacket
    {
        public int Id => 0x01;

        public ReasonCode ReasonCode { get; set; }

        public P01State()
        {

        }

        public P01State(ReasonCode reasonCode)
        {
            ReasonCode = reasonCode;
        }

        public void Read(PacketBuffer buffer)
        {
            ReasonCode = (ReasonCode)buffer.ReadInt();
        }

        public void Write(PacketBuffer buffer)
        {
            buffer.WriteInt((int)ReasonCode);
        }

        public void Handle(INetHandler netHandler)
        {
            netHandler.HandleP01State(this);
        }
    }

    public enum ReasonCode
    {
        Ok,
        UsernameTaken
    }
}
