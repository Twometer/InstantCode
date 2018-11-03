using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using InstantCode.Protocol.IO;

namespace InstantCode.Client.GUI.Model
{
    public class ServerList
    {
        private static readonly string SavePath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "InstantCode", "srvlist.dat");

        private readonly List<ServerEntry> entries = new List<ServerEntry>();
        public IList<ServerEntry> Entries => entries.AsReadOnly();

        public void AddServer(ServerEntry entry)
        {
            entries.Add(entry);
            Save();
        }

        public void Save()
        {
            EnsureDirectoryExists();
            var secureData = ProtectedData.Protect(Serialize(), null, DataProtectionScope.CurrentUser);
            File.WriteAllBytes(SavePath, secureData);
        }

        public void Load()
        {
            if (!File.Exists(SavePath))
                return;
            var plaintextData =
                ProtectedData.Unprotect(File.ReadAllBytes(SavePath), null, DataProtectionScope.CurrentUser);
            Deserialize(plaintextData);
        }

        private void Deserialize(byte[] data)
        {
            entries.Clear();
            var buf = new PacketBuffer(data);
            var count = buf.ReadInt();
            for (var i = 0; i < count; i++)
            {
                var name = buf.ReadString();
                var ip = buf.ReadString();
                var username = buf.ReadString();
                var password = buf.ReadString();
                entries.Add(new ServerEntry(name, ip, username, password));
            }
        }

        private byte[] Serialize()
        {
            var buf = new PacketBuffer();
            buf.WriteInt(entries.Count);
            foreach (var entry in entries)
            {
                buf.WriteString(entry.Name);
                buf.WriteString(entry.Ip);
                buf.WriteString(entry.Username);
                buf.WriteString(entry.Password);
            }
            return buf.ToArray();
        }

        private static void EnsureDirectoryExists()
        {
            var dir = new FileInfo(SavePath).Directory;
            if (!dir.Exists)
                dir.Create();
        }

    }
}
