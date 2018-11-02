using System.Net;
using System.Net.Sockets;

namespace InstantCode.Server.Utility
{
    public class Util
    {
        public static string GetIp(TcpClient tcpClient)
        {
            return ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString();
        }
    }
}
