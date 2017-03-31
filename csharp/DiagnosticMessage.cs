using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AntlrServer
{
    public class DiagnosticMessage
    {
        public string Message { get; set; }
        public string Symbol { get; set; }
        public int Line { get; set; }
        public int Character { get; set; }

        public int Length { get; set;}
    }
}