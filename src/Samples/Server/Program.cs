using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server
{
    public class Program
    {
        private static readonly string[] SupportedSampleTransports = {"tcp", "namedpipe", "http"};

        public static void Main(string[] args)
        {
            Run(args);
        }

        private static void Run(string[] args)
        {
            if (args == null || !args.Any(x => x.ToLowerInvariant().Contains("-t:")))
            {
                DisplayHelp();
                return;
            }

            var transport = args.First(x => x.StartsWith("-t")).Split(':')[1];
            if (string.IsNullOrWhiteSpace(transport) || !SupportedSampleTransports.Contains(transport, StringComparer.OrdinalIgnoreCase))
            {
                DisplayHelp();
                return;
            }

            if (transport.Equals("tcp", StringComparison.OrdinalIgnoreCase))
            {
                new TaskFactoryServerSample().Run();
            }
            else if (transport.Equals("namedpipe", StringComparison.OrdinalIgnoreCase))
            {
                new NamedPipeSimpleServerSample().Run();
            }
            else if (transport.Equals("http", StringComparison.OrdinalIgnoreCase))
            {
                new HttpServerSample().Run();
            }
            else
            {
                DisplayHelp();
            }
        }

        private static void DisplayHelp()
        {
            Console.WriteLine(@"
Usage: 
    Server.exe 
        will diplay help information 

    Server.exe -t:<transport>
        will run server with specified arguments

Options:
    -t (transport): 
        tcp - tcp transport will be used (host - ""localhost"", port - 9090)
        namedpipe - namedpipe transport will be used (pipe address - "".test"")
        http - http transport will be used (http address - ""localhost:9090"")
        
Sample:
    Server.exe -t:tcp 
");
        }
    }
}
