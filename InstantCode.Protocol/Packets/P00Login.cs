﻿using InstantCode.Protocol.Handler;
using InstantCode.Protocol.IO;

namespace InstantCode.Protocol.Packets
{
    public class P00Login : IPacket
    {
        public int Id => 0x00;

        public string Username { get; set; }

        public P00Login()
        {

        }

        public P00Login(string username)
        {
            Username = username;
        }

        public void Read(PacketBuffer buffer)
        {
            Username = buffer.ReadString();
        }

        public void Write(PacketBuffer buffer)
        {
            buffer.WriteString(Username);
        }

        public void Handle(INetHandler netHandler)
        {
            netHandler.HandleP00Login(this);
        }
    }
}
