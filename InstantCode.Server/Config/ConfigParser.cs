using System;
using System.IO;
using Newtonsoft.Json;

namespace InstantCode.Server.Config
{
    public class ConfigParser
    {
        private readonly string path;
        private string json;

        private ConfigParser(string path, string json)
        {
            this.path = path;
            this.json = json;
        }

        public ConfigParser EnsureCreated()
        {
            if (json == "")
            {
                Console.WriteLine("Config file not found, creating deafult config...");
                json = JsonConvert.SerializeObject(new ServerConfig());
                File.WriteAllText(path, json);
            }
            return this;
        }

        public ServerConfig Parse()
        {
            return JsonConvert.DeserializeObject<ServerConfig>(json);
        }

        public static ConfigParser FromFile(string file)
        {
            return new ConfigParser(file, File.Exists(file) ? File.ReadAllText(file) : "");
        }
    }
}
