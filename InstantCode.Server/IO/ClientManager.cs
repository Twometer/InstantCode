using System.Collections.Generic;
using System.Net.Sockets;

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
    }
}
