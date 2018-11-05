using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstantCode.Client.Model
{
    public class DocumentModification
    {
        public string File { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public string Data { get; set; }
    }
}
