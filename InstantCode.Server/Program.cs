using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using InstantCode.Server.Config;
using InstantCode.Server.Crypto;
using InstantCode.Server.IO;
using InstantCode.Server.Utility;

namespace InstantCode.Server
{
    internal class Program
    {
        private const string Tag = "Main";

        private static readonly string ConfigPath = Path.Combine(Environment.CurrentDirectory, "config.json");

        private static void Main(string[] args)
        {
            var config = ConfigParser.FromFile(ConfigPath).EnsureCreated().Parse();
            CredentialStore.Store(config.Password);

            var listener = new TcpListener(IPAddress.Any, config.Port);
            listener.Start();

            Log.I(Tag, "InstantCode server started on port " + config.Port);

            while (true)
            {
                var client = listener.AcceptTcpClient();
                ClientManager.AcceptClient(client);
            }
        }
    }
}
