using System.Threading;
using InstantCode.Protocol;

namespace InstantCode.Client.Network
{
    public class PacketAwaitItem
    {
        public ManualResetEvent WaitHandle { get; } = new ManualResetEvent(false);
        public int PacketId { get; set; }
        public IPacket Packet { get; set; }

        public void Reset()
        {
            WaitHandle.Reset();
        }

    }
}
