using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using System.IO;

namespace AntlrServer
{    
    public class DOTLexerErrorListener : IAntlrErrorListener<int>
    {                                
        public List<DiagnosticMessage> Messages { get; private set; }        
        public DOTLexerErrorListener()
        {            
            Messages = new List<DiagnosticMessage>();
        }

        public void SyntaxError(IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            DiagnosticMessage dm = new DiagnosticMessage()
            {
                Message = msg,
                Line = line - 1,
                Character = charPositionInLine,
                Symbol = "",
                Length = 1
            };            

            if(offendingSymbol != 0)
            {                
                dm.Symbol = recognizer.Vocabulary.GetDisplayName(offendingSymbol);                
            }

            Messages.Add(dm);
        }        
    }
}