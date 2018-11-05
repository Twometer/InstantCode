using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstantCode.Protocol.Packets;

namespace InstantCode.Client.Model
{
    public class Session
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string[] Participants { get; set; }

        public Dictionary<string, CursorPosition> CursorPositions { get; set; } = new Dictionary<string, CursorPosition>();

        public void UpdateCursors(P08CursorPosition positionPacket)
        {
            var pos = new CursorPosition { File = positionPacket.File, Index = positionPacket.NewIndex, User = positionPacket.Sender };
            if (CursorPositions.ContainsKey(pos.User))
                CursorPositions[pos.User] = pos;
            else CursorPositions.Add(pos.User, pos);
        }
    }
}
