using InstantCode.Protocol.IO;

namespace InstantCode.Server.Model
{
    public class ClientData
    {
        public string Username { get; set; }
        public int CurrentSessionId { get; set; }
        public Session CurrentSession => SessionManager.Find(CurrentSessionId);
    }
}
