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
            foreach (var client in ClientManager.ConnectedClients)
            {
                if (!string.Equals(client.ClientData.Username, login.Username, StringComparison.OrdinalIgnoreCase)) continue;
                Log.I(Tag, "Rejected login attempt for " + client.Ip + ": Username already taken");
                clientHandler.SendPacket(new P01State(ReasonCode.UsernameTaken));
                return;
            }

            clientHandler.ClientData.Username = login.Username;
            clientHandler.SendPacket(new P01State(ReasonCode.Ok));
        }

        public void HandleP01State(P01State state)
        {
            throw new NotImplementedException();
        }
    }
}
