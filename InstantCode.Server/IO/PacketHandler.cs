using System;
using System.IO;
using System.Linq;
using InstantCode.Protocol.Handler;
using InstantCode.Protocol.IO;
using InstantCode.Protocol.Packets;
using InstantCode.Server.Model;
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
                Log.I(Tag, $"Rejected login attempt for {client.Ip}: Username already taken");
                clientHandler.SendPacket(new P01State(ReasonCode.UsernameTaken));
                return;
            }

            clientHandler.ClientData.Username = p00Login.Username;
            clientHandler.SendPacket(new P01State(ReasonCode.Ok));
            clientHandler.SendPacket(new P0AUserList(ClientManager.ConnectedClients.Select(c => c.ClientData.Username).Where(c => !string.IsNullOrWhiteSpace(c)).Distinct().ToList()));
            Log.I(Tag, $"{clientHandler.Ip} logged in with username '{p00Login.Username}'");
        }

        public void HandleP01State(P01State p01State)
        {
            throw new NotImplementedException();
        }

        public void HandleP02NewSession(P02NewSession p02NewSession)
        {
            var session = SessionManager.CreateNew(p02NewSession.ProjectName, p02NewSession.Participants);
            ClientManager.ForSession(session, handler => handler.SendPacket(p02NewSession));
            clientHandler.SendPacket(new P01State(ReasonCode.Ok, session.Id));
            Log.I(Tag, $"{clientHandler.ClientData.Username} created a new session {session.Name} with Id {session.Id:X} and participants [{string.Join(',', session.Participants)}]");
        }

        public void HandleP03CloseSession(P03CloseSession p03CloseSession)
        {
            if (p03CloseSession.SessionId != clientHandler.ClientData.CurrentSessionId)
            {
                clientHandler.SendPacket(new P01State(ReasonCode.NoPermission));
                return;
            }
            ClientManager.ForSession(SessionManager.Find(p03CloseSession.SessionId), handler => handler.SendPacket(p03CloseSession));
            clientHandler.SendPacket(new P01State(ReasonCode.Ok));
            Log.I(Tag, $"{clientHandler.ClientData.Username} closed session {p03CloseSession.SessionId:X}");
        }

        public void HandleP04OpenStream(P04OpenStream p04OpenStream)
        {
            var fileStream = File.Open(clientHandler.ClientData.CurrentSession.DataPath, FileMode.Create, FileAccess.Write);
            clientHandler.ClientData.CurrentSession.DataTransmission = new DataTransmission(fileStream, p04OpenStream.DataLength);
        }

        public void HandleP05StreamData(P05StreamData p05StreamData)
        {
            clientHandler.ClientData.CurrentSession.DataTransmission?.Stream?.Write(p05StreamData.Data);
        }

        public void HandleP06CloseStream(P06CloseStream p06CloseStream)
        {
            clientHandler.ClientData.CurrentSession.DataTransmission?.Close();
        }

        public void HandleP07CodeChange(P07CodeChange p07CodeChange)
        {
            ClientManager.ForSession(SessionManager.Find(clientHandler.ClientData.CurrentSessionId), handler => handler.SendPacket(p07CodeChange));
        }

        public void HandleP08CursorPosition(P08CursorPosition p08CursorPosition)
        {
            ClientManager.ForSession(SessionManager.Find(clientHandler.ClientData.CurrentSessionId), handler => handler.SendPacket(p08CursorPosition));
        }

        public void HandleP09Save(P09Save p09Save)
        {
            ClientManager.ForSession(SessionManager.Find(clientHandler.ClientData.CurrentSessionId), handler => handler.SendPacket(p09Save));
        }

        public void HandleP0AUserList(P0AUserList p0AUserList)
        {
            throw new NotImplementedException();
        }
    }
}
