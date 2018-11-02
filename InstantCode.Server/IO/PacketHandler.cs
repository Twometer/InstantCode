using System;
using System.Collections.Generic;
using System.Text;
using InstantCode.Protocol.Handler;
using InstantCode.Protocol.Packets;

namespace InstantCode.Server.IO
{
    public class PacketHandler : INetHandler
    {
        private readonly ClientHandler clientHandler;

        public PacketHandler(ClientHandler clientHandler)
        {
            this.clientHandler = clientHandler;
        }

        public void HandleP00Login(P00Login login)
        {
            throw new NotImplementedException();
        }
    }
}
