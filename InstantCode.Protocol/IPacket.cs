using System;
using System.Collections.Generic;
using System.Text;

namespace InstantCode.Protocol
{
    interface IPacket
    {

        void Read(PacketBuffer buffer);

        void Write(PacketBuffer buffer);

    }
}
