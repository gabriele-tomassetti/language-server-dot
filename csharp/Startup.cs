using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Antlr4.Runtime;
using System.IO;
using static AntlrServer.DOTParser;
using System.Text;
using Microsoft.AspNetCore.Routing;

namespace AntlrServer
{
    public class Startup    
    {              
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
        
            var routeBuilder = new RouteBuilder(app);                
            
            routeBuilder.MapPost("parse", context =>
            {                
                string text = "";
                using(StreamReader reader = new StreamReader(context.Request.Body))
                {
                    text = reader.ReadToEnd();
                }

                AntlrInputStream inputStream = new AntlrInputStream(text);
                DOTLexer lexer = new DOTLexer(inputStream);
                CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
                DOTParser parser = new DOTParser(commonTokenStream);            
                // the listener gathers the names for the hover information
                DOTLanguageListener listener = new DOTLanguageListener();
                
                DOTErrorListener errorListener = new DOTErrorListener();
                DOTLexerErrorListener lexerErrorListener = new DOTLexerErrorListener();
                
                lexer.RemoveErrorListeners();
                lexer.AddErrorListener(lexerErrorListener);   
                parser.RemoveErrorListeners();
                parser.AddErrorListener(errorListener);
                
                GraphContext graph = parser.graph(); 

                ParseTreeWalker.Default.Walk(listener, graph);                

                StringBuilder json = new StringBuilder();
                json.Append("{");                
                json.Append("\"errors\": [");

                json.Append(convertMessagesToJson(lexerErrorListener.Messages));
                json.Append(convertMessagesToJson(errorListener.Messages));
                
                if(lexerErrorListener.Messages.Count + errorListener.Messages.Count > 0)
                    json.Remove(json.Length-2, 1);

                json.Append("], ");              
                json.Append("\"names\": [");
                json.Append(convertNamesToJson(listener.Names));
                json.Append("]");
                json.Append("}");
                
                return context.Response.WriteAsync(json.ToString());
            });
        
            var routes = routeBuilder.Build();

            app.UseRouter(routes);
        }

        public string convertMessagesToJson(List<DiagnosticMessage> messages)
        {
            StringBuilder json = new StringBuilder();
            
            foreach(var message in messages)
            {
                json.Append("{ ");
                json.Append($"\"message\" : \"{message.Message}\", ");
                json.Append($"\"line\" : {message.Line}, ");
                json.Append($"\"character\" : {message.Character}, ");
                // yes, we don't actually use it in the server, but it could be useful
                // for instance to find all errors relative to a symbol
                json.Append($"\"symbol\" : \"{message.Symbol}\", ");
                json.Append($"\"length\" : {message.Length}");
                json.Append("}, ");
            }                        

            return json.ToString();
        }

        public string convertNamesToJson(List<Name> names)
        {
            StringBuilder json = new StringBuilder();
            
            foreach(var name in names)
            {                
                json.Append("{ ");
                json.Append($"\"text\" : \"{name.Text}\", ");
                json.Append($"\"line\" : {name.Line}, ");
                json.Append($"\"start\" : {name.Start}, ");
                json.Append($"\"end\" : {name.End} ");
                json.Append("}, ");
            }
            
            if(names.Count > 0)
                json.Remove(json.Length-2, 1);                                 

            return json.ToString();
        }
    }
}
