﻿using System;
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
            foreach (var client in ClientManager.ConnectedClients)
                client.SendPacket(new P0AUserList(ClientManager.ConnectedClients.Select(c => c.ClientData.Username).Where(c => !string.IsNullOrWhiteSpace(c) && c != client.ClientData.Username).Distinct().ToList()));
            Log.I(Tag, $"{clientHandler.Ip} logged in with username '{p00Login.Username}'");
        }

        public void HandleP01State(P01State p01State)
        {
            throw new NotImplementedException();
        }

        public void HandleP02NewSession(P02NewSession p02NewSession)
        {
            var session = SessionManager.CreateNew(p02NewSession.ProjectName, p02NewSession.Participants);
            ClientManager.AssignSession(session);
            ClientManager.ForSession(clientHandler, session, handler => handler.SendPacket(p02NewSession));
            clientHandler.ClientData.CurrentSessionId = session.Id;
            clientHandler.SendPacket(new P01State(ReasonCode.Ok, session.Id));
            Log.I(Tag, $"{clientHandler.ClientData.Username} created a new session {session.Name} with id {session.Id:X} and participants [{string.Join(',', session.Participants)}]");
        }

        public void HandleP03CloseSession(P03CloseSession p03CloseSession)
        {
            if (p03CloseSession.SessionId != clientHandler.ClientData.CurrentSessionId)
            {
                clientHandler.SendPacket(new P01State(ReasonCode.NoPermission));
                return;
            }
            ClientManager.ForSession(clientHandler, SessionManager.Find(p03CloseSession.SessionId), handler => handler.SendPacket(p03CloseSession));
            clientHandler.SendPacket(new P01State(ReasonCode.Ok));
            Log.I(Tag, $"{clientHandler.ClientData.Username} closed session {p03CloseSession.SessionId:X}");
        }

        public void HandleP04OpenStream(P04OpenStream p04OpenStream)
        {
            var fileStream = File.Open(clientHandler.ClientData.CurrentSession.DataPath, FileMode.Create, FileAccess.Write);
            clientHandler.ClientData.CurrentSession.DataTransmission = new DataTransmission(fileStream, p04OpenStream.DataLength);
            clientHandler.SendPacket(new P01State(ReasonCode.Ok));
        }

        public void HandleP05StreamData(P05StreamData p05StreamData)
        {
            clientHandler.ClientData.CurrentSession.DataTransmission?.Stream?.Write(p05StreamData.Data);
        }

        public void HandleP06CloseStream(P06CloseStream p06CloseStream)
        {
            clientHandler.ClientData.CurrentSession.DataTransmission?.Close();
            Log.I(Tag, $"{clientHandler.ClientData.Username} finished uploading project data. Transmitting to rest of session...");
            using (var fileStream = File.OpenRead(clientHandler.ClientData.CurrentSession.DataPath))
            {
                var streamInit = new P04OpenStream(clientHandler.ClientData.CurrentSession.DataTransmission.Length);
                ClientManager.ForSession(clientHandler, clientHandler.ClientData.CurrentSession, client => client.SendPacket(streamInit));

                var buffer = new byte[8192];
                while (true)
                {
                    var read = fileStream.Read(buffer, 0, buffer.Length);
                       if (read <= 0)
                        break;

                    var sendBuffer = new byte[read];
                    Array.Copy(buffer, 0, sendBuffer, 0, sendBuffer.Length);

                    var dataPacket = new P05StreamData(sendBuffer);
                    ClientManager.ForSession(clientHandler, clientHandler.ClientData.CurrentSession, client => client.SendPacket(dataPacket));
                }
                ClientManager.ForSession(clientHandler, clientHandler.ClientData.CurrentSession, client => client.SendPacket(new P06CloseStream()));
            }
            Log.I(Tag, $"Successfully transmitted {clientHandler.ClientData.Username}'s project to their session.");
        }

        public void HandleP07CodeChange(P07CodeChange p07CodeChange)
        {
            ClientManager.ForSession(clientHandler, clientHandler.ClientData.CurrentSession, handler => handler.SendPacket(p07CodeChange));
        }

        public void HandleP08CursorPosition(P08CursorPosition p08CursorPosition)
        {
            ClientManager.ForSession(clientHandler, clientHandler.ClientData.CurrentSession, handler => handler.SendPacket(p08CursorPosition));
        }

        public void HandleP09Save(P09Save p09Save)
        {
            ClientManager.ForSession(clientHandler, clientHandler.ClientData.CurrentSession, handler => handler.SendPacket(p09Save));
        }

        public void HandleP0AUserList(P0AUserList p0AUserList)
        {
            throw new NotImplementedException();
        }

    }
}
