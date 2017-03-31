using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using System.IO;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace AntlrServer
{    
    public class DOTLanguageListener : DOTBaseListener
    {
        public List<Name> Names { get; private set; }                
        public DOTLanguageListener()
        {            
            this.Names = new List<Name>();
        }

        public override void ExitId(DOTParser.IdContext context)
        {
            string name = "";
             
            if(context.Parent.GetType().Name == "Node_idContext")
                name = "(Node) ";
            
            if(context.Parent.GetType().Name == "SubgraphContext")
                name = "(Subgraph) ";

            if(context.Parent.GetType().Name == "GraphContext")                
                name = "(Graph) ";            
                         

            if(!String.IsNullOrEmpty(name))
            {       
                Names.Add(new Name() {
                    Text = name + context.GetText(),
                    Line = context.Stop.Line - 1,
                    Start =  context.Start.Column,
                    End = context.Start.Column + context.GetText().Length
                });
            }
        }

    }
}