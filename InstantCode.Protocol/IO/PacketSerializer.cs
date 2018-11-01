using System;
using System.Collections.Generic;
using System.Text;

namespace InstantCode.Protocol.IO
{
    public class PacketSerializer
    {
        public static byte[] Serialize(IPacket packet)
        {
            var buffer = new PacketBuffer();
            packet.Write(buffer);
            return buffer.ToArray();
        }
    }
}
