using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using System.IO;

namespace AntlrServer
{    
    public class DOTErrorListener : BaseErrorListener
    {                
        public List<DiagnosticMessage> Messages { get; private set; }
        
        public DOTErrorListener()
        {
            Messages = new List<DiagnosticMessage>();
        }

        public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {            
            DiagnosticMessage dm = new DiagnosticMessage()
            {
                Message = msg,
                Line = line - 1,
                Character = charPositionInLine,
                Symbol = "",
                Length = 0
            };

            if(offendingSymbol != null)
            {
                dm.Symbol = offendingSymbol.Text;
                dm.Length = offendingSymbol.StopIndex - offendingSymbol.StartIndex + 1;
            }

            Messages.Add(dm);            
        }
    }
}