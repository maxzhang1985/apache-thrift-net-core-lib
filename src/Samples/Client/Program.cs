using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client
{
    public class Program
    {
        private static readonly string[] SupportedSampleTransports = { "tcp", "namedpipe", "http" };

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
            if (string.IsNullOrWhiteSpace(transport) ||
                !SupportedSampleTransports.Contains(transport, StringComparer.OrdinalIgnoreCase))
            {
                DisplayHelp();
                return;
            }

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

    Client.exe -t:<transport>
        will run client with specified arguments

Options:
    -t (transport): 
        tcp - tcp transport will be used (host - ""localhost"", port - 9090)
        namedpipe - namedpipe transport will be used (pipe address - "".test"")
        http - http transport will be used (address - ""localhost:9090"")

Sample:
    Client.exe -t:tcp
");
        }
    }
}
