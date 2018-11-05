using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstantCode.Client.Model
{
    public class CursorPosition
    {
        public string File { get; set; }
        public string User { get; set; }
        public int Index { get; set; }
    }
}
