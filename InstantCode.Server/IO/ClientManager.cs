using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using InstantCode.Protocol.Packets;
using InstantCode.Server.Model;

namespace InstantCode.Server.IO
{
    public class ClientManager
    {
        public static List<ClientHandler> ConnectedClients { get; } = new List<ClientHandler>();

        public static void AcceptClient(TcpClient client)
        {
            var handler = new ClientHandler(client);
            handler.StartReading();
            lock (ConnectedClients)
                ConnectedClients.Add(handler);
        }

        public static void RemoveClient(ClientHandler handler)
        {
            lock (ConnectedClients)
                ConnectedClients.Remove(handler);
        }

        public static void AssignSession(Session session)
        {
            foreach (var client in ConnectedClients)
                if (session.Participants.Any(name => client.ClientData.Username == name))
                {
                    client.ClientData.CurrentSessionId = session.Id;
                    client.SendPacket(new P01State(ReasonCode.SessionJoined, session.Id));
                }
        }

        public static void ForSession(Session session, Action<ClientHandler> action)
        {
            foreach (var client in ConnectedClients)
                if (session.Participants.Any(name => client.ClientData.Username == name))
                    action(client);
        }
    }
}
