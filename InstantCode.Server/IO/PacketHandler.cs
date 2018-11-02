using System;
using System.Collections.Generic;
using System.Text;
using InstantCode.Protocol.Handler;
using InstantCode.Protocol.Packets;
using InstantCode.Server.Utility;

namespace InstantCode.Server.IO
{
    public class PacketHandler : INetHandler
    {
        private const string Tag = "PacketHandler";

        private readonly ClientHandler clientHandler;

        public PacketHandler(ClientHandler clientHandler)
        {
            this.clientHandler = clientHandler;
        }

        public void HandleP00Login(P00Login login)
        {
            Log.I(Tag, "User " + login.Username + " logged in");
            clientHandler.SendPacket(new P00Login() {Username = login.Username});
        }
    }
}
