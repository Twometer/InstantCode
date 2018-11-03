using System;

namespace InstantCode.Client.GUI.Model
{
    [Serializable]
    public class ServerEntry
    {
        public string Name { get; }
        public string Ip { get; }
        public string Username { get; }
        public string Password { get; }

        public ServerEntry(string name, string ip, string username, string password)
        {
            Name = name;
            Ip = ip;
            Username = username;
            Password = password;
        }
    }
}
