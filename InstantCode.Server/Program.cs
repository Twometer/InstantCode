using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using InstantCode.Server.Config;
using InstantCode.Server.Crypto;
using InstantCode.Server.IO;

namespace InstantCode.Server
{
    internal class Program
    {
        private static readonly string ConfigPath = Path.Combine(Environment.CurrentDirectory, "config.json");

        private static void Main(string[] args)
        {
            var config = ConfigParser.FromFile(ConfigPath).EnsureCreated().Parse();
            CredentialStore.Store(config.Password);

            var listener = new TcpListener(IPAddress.Any, config.Port);
            listener.Start();

            Console.WriteLine("Started InstantCode server on port " + config.Port);

            while (true)
            {
                var client = listener.AcceptTcpClient();
                Console.WriteLine(((IPEndPoint)client.Client.RemoteEndPoint).Address + " connected");
                ClientManager.AcceptClient(client);
            }
        }
    }
}
