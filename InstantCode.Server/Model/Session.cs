using System.IO;
using InstantCode.Protocol.IO;

namespace InstantCode.Server.Model
{
    public class Session
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string[] Participants { get; set; }
        public DataTransmission DataTransmission { get; set; }
        public string DataPath => Path.Combine(Program.UserDirectory, $"data-{Name}-{Id:X}.dat");
    }
}
