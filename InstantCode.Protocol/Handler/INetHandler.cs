using System;
using System.Collections.Generic;
using System.Text;
using InstantCode.Protocol.Packets;

namespace InstantCode.Protocol.Handler
{
    public interface INetHandler
    {

        void HandleP00Login(P00Login login);
        void HandleP01State(P01State state);

    }
}
