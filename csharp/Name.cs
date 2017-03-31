using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AntlrServer
{
    public class Name
    {
        public string Text { get; set; }        
        public int Line { get; set; }
        public int Start { get; set; }
        public int End { get; set; }     
    }
}