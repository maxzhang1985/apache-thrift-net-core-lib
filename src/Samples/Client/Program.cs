using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Serilog.Core;
using Thrift;
using Thrift.Protocols;
using Thrift.Samples;
using Thrift.Transports;
using Thrift.Transports.Client;

namespace Client
{
    public class Program
    {
        static readonly Logger Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.WithThreadId()
            .WriteTo.ColoredConsole(
                outputTemplate:
                "{Timestamp:yyyy-MM-dd HH:mm:ss:fff} [{Level}] [ThreadId:{ThreadId}] {SourceContext:l} {Message}{NewLine}{Exception}")
            .CreateLogger();

        private enum Transport
        {
            Tcp,
            NamedPipe,
            Http,
            TcpTls
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
        tcptls - tcp transport with tls will be used (host - ""localhost"", port - 9090)
        namedpipe - namedpipe transport will be used (pipe address - "".test"")
        http - http transport will be used (address - ""http://localhost:9090"")

Sample:
    Client.exe -t:tcp
");
        }

        public static void Main(string[] args)
        {
            using (var source = new CancellationTokenSource())
            {
                RunAsync(args, source.Token).GetAwaiter().GetResult();
            }
        }

        private static async Task RunAsync(string[] args, CancellationToken cancellationToken)
        {
            if (args == null || !args.Any(x => x.ToLowerInvariant().Contains("-t:")))
            {
                DisplayHelp();
                return;
            }

            TClientTransport clientTransport = null;

            var transport = args.First(x => x.StartsWith("-t")).Split(':')[1];
            Transport selectedTransport;
            if (Enum.TryParse(transport, true, out selectedTransport))
            {
                switch (selectedTransport)
                {
                    case Transport.Tcp:
                        clientTransport = new TSocketClientTransport(IPAddress.Loopback, 9090);
                        break;
                    case Transport.NamedPipe:
                        clientTransport = new TNamedPipeClientTransport(".test");
                        break;
                    case Transport.Http:
                        clientTransport = new THttpClientTransport(new Uri("http://localhost:9090"));
                        break;
                    case Transport.TcpTls:
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var protocol = new TBinaryProtocol(clientTransport);
                await RunClientAsync(protocol, cancellationToken);
            }
            else
            {
                DisplayHelp();
            }
        }

        private static async Task RunClientAsync(TProtocol protocol, CancellationToken cancellationToken)
        {
            
            try
            {
                var client = new Calculator.Client(protocol);

                await client.OpenTransportAsync(cancellationToken);

                try
                {
                    // Async version

                    Logger.Information("PingAsync()");
                    await client.PingAsync(cancellationToken);

                    Logger.Information("AddAsync(1,1)");
                    var sum = await client.AddAsync(1, 1, cancellationToken);
                    Logger.Information($"AddAsync(1,1)={sum}");
                    
                    var work = new Work
                    {
                        Op = Operation.Divide,
                        Num1 = 1,
                        Num2 = 0
                    };

                    try
                    {
                        Logger.Information("CalculateAsync(1)");
                        await client.CalculateAsync(1, work, cancellationToken);
                        Logger.Information("Whoa we can divide by 0");
                    }
                    catch (InvalidOperation io)
                    {
                        Logger.Information(io, "Invalid operation: " + io.Why);
                    }

                    work.Op = Operation.Substract;
                    work.Num1 = 15;
                    work.Num2 = 10;

                    try
                    {
                        Logger.Information("CalculateAsync(1)");
                        var diff = await client.CalculateAsync(1, work, cancellationToken);
                        Logger.Information($"15-10={diff}");
                    }
                    catch (InvalidOperation io)
                    {
                        Logger.Information(io, "Invalid operation: " + io.Why);
                    }

                    Logger.Information("GetStructAsync(1)");
                    var log = await client.GetStructAsync(1, cancellationToken);
                    Logger.Information($"Check log: {log.Value}");

                    Logger.Information("ZipAsync() with delay 100mc on server side");
                    await client.ZipAsync(cancellationToken);

                }
                finally
                {
                    protocol.Transport.Close();
                }
            }
            catch (TApplicationException x)
            {
                Logger.Error(x, x.StackTrace);
            }
        }
    }
}
