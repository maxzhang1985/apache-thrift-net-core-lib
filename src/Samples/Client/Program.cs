using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client
{
    public class Program
    {
        private static readonly string[] SupportedSampleTransports = { "tcp", "namedpipe", "http" };
        private static readonly string[] SupportedSampleServers = { "simple" };

        public static void Main(string[] args)
        {
            Run(args);
        }

        private static void Run(string[] args)
        {
            if (args == null || args.Length == 1
                || !args.Any(x => x.ToLowerInvariant().Contains("-t:"))
                || !args.Any(x => x.ToLowerInvariant().Contains("-s:")))
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

            var server = args.First(x => x.StartsWith("-s")).Split(':')[1];
            if (string.IsNullOrWhiteSpace(server) || !SupportedSampleServers.Contains(server, StringComparer.OrdinalIgnoreCase))
            {
                DisplayHelp();
                return;
            }

            if (server.Equals("simple", StringComparison.OrdinalIgnoreCase))
            {
                if (transport.Equals("tcp", StringComparison.OrdinalIgnoreCase))
                {
                    new SimpleClientSample().Run();
                }
                else if (transport.Equals("namedpipe", StringComparison.OrdinalIgnoreCase))
                {
                    new NamedPipeSimpleClientSample().Run();
                }
                else if (transport.Equals("http", StringComparison.OrdinalIgnoreCase))
                {
                    new HttpClientSample().Run();
                }
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
    Client.exe 
        will diplay help information 

    Client.exe -t:<transport> -s:<server>
        will run client with specified arguments

Options:
    -t (transport): 
        tcp - tcp transport will be used (host - ""localhost"", port - 9090)
        namedpipe - namedpipe transport will be used (pipe address - "".test"")
        http - http transport will be used (address - ""localhost:9090"")
        
    -s (server):
        simple - simple server will be used 

Sample:
    Client.exe -transport:tcp -server:simple
");
        }
    }
}
