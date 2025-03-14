﻿using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using InstantCode.Protocol.Crypto;
using InstantCode.Server.Config;
using InstantCode.Server.IO;
using InstantCode.Server.Utility;

namespace InstantCode.Server
{
    internal class Program
    {
        private const string Tag = "Main";

        private static readonly string ConfigPath = Path.Combine(Environment.CurrentDirectory, "config.json");

        public static readonly string UserDirectory = Path.Combine(Environment.CurrentDirectory, "users");

        private static void Main(string[] args)
        {
            Log.I(Tag, "Loading configuration");
            var config = ConfigParser.FromFile(ConfigPath).EnsureCreated().Parse();
            CredentialStore.Store(config.Password);

            SetupFileSystem();

            var listener = new TcpListener(IPAddress.Any, config.Port);
            listener.Start();

            Log.I(Tag, "InstantCode server started on port " + config.Port);

            while (true)
            {
                var client = listener.AcceptTcpClient();
                ClientManager.AcceptClient(client);
            }
        }

        private static void SetupFileSystem()
        {
            Log.I(Tag, "Setting up file system");
            if (!Directory.Exists(UserDirectory))
                Directory.CreateDirectory(UserDirectory);
        }
    }
}
