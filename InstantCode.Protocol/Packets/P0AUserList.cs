using System;
using System.Collections.Generic;
using System.Text;
using InstantCode.Protocol.Handler;
using InstantCode.Protocol.IO;

namespace InstantCode.Protocol.Packets
{
    public class P0AUserList : IPacket
    {
        public int Id => 0x0A;

        public List<string> UserList { get; set; }

        public P0AUserList()
        {
            UserList = new List<string>();
        }

        public P0AUserList(List<string> userList)
        {
            UserList = userList;
        }

        public void Read(PacketBuffer buffer)
        {
            UserList.Clear();
            var len = buffer.ReadInt();
            for(var i = 0; i < len; i++)
                UserList.Add(buffer.ReadString());
        }

        public void Write(PacketBuffer buffer)
        {
            buffer.WriteInt(UserList.Count);
            foreach(var user in UserList)
                buffer.WriteString(user);
        }

        public void Handle(INetHandler netHandler)
        {
            netHandler.HandleP0AUserList(this);
        }
    }
}
