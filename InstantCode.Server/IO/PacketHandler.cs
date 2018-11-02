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

        public void HandleP00Login(P00Login p00Login)
        {
            foreach (var client in ClientManager.ConnectedClients)
            {
                if (!string.Equals(client.ClientData.Username, p00Login.Username, StringComparison.OrdinalIgnoreCase)) continue;
                Log.I(Tag, "Rejected login attempt for " + client.Ip + ": Username already taken");
                clientHandler.SendPacket(new P01State(ReasonCode.UsernameTaken));
                return;
            }

            clientHandler.ClientData.Username = p00Login.Username;
            clientHandler.SendPacket(new P01State(ReasonCode.Ok));
            Log.I(Tag, clientHandler.Ip + " logged in with username '" + p00Login.Username + "'");
        }

        public void HandleP01State(P01State p01State)
        {
            throw new NotImplementedException();
        }

        public void HandleP02NewSession(P02NewSession p02NewSession)
        {
            throw new NotImplementedException();
        }

        public void HandleP03CloseSession(P03CloseSession p03CloseSession)
        {
            throw new NotImplementedException();
        }

        public void HandleP04OpenStream(P04OpenStream p04OpenStream)
        {
            throw new NotImplementedException();
        }

        public void HandleP05StreamData(P05StreamData p05StreamData)
        {
            throw new NotImplementedException();
        }

        public void HandleP06CloseStream(P06CloseStream p06CloseStream)
        {
            throw new NotImplementedException();
        }

        public void HandleP07CodeChange(P07CodeChange p07CodeChange)
        {
            throw new NotImplementedException();
        }

        public void HandleP08CursorPosition(P08CursorPosition p08CursorPosition)
        {
            throw new NotImplementedException();
        }

        public void HandleP09Save(P09Save p09Save)
        {
            throw new NotImplementedException();
        }
    }
}
