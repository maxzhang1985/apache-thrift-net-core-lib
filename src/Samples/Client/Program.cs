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
            Buffered,
            Framed,
            TcpTls
        }

        private enum Protocol
        {
            Binary,
            Compact,
            Json,
        }

        private static void DisplayHelp()
        {
            Console.WriteLine(@"
Usage: 
    Client.exe -h
        will diplay help information 

    Client.exe -t:<transport> -p:<protocol>
        will run client with specified arguments (tcp transport and binary protocol by default)

Options:
    -t (transport): 
        tcp - (default) tcp transport will be used (host - ""localhost"", port - 9090)
        namedpipe - namedpipe transport will be used (pipe address - "".test"")
        http - http transport will be used (address - ""http://localhost:9090"")
        buffered - buffered transport over tcp will be used (host - ""localhost"", port - 9090)

    -p (protocol): 
        binary - (default) binary protocol will be used
        compact - compact protocol will be used
        json - json protocol will be used

Sample:
    Client.exe -t:tcp -p:binary
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
            args = args ?? new string[0];

            if (args.Any(x => x.StartsWith("-h", StringComparison.OrdinalIgnoreCase)))
            {
                DisplayHelp();
                return;
            }

            var clientTransport = GetTransport(args);

            Logger.Information($"Selected client transport: {clientTransport}");

            var clientProtocol = GetProtocol(args, clientTransport);

            Logger.Information($"Selected client protocol: {clientProtocol}");

            await RunClientAsync(clientProtocol, cancellationToken);
        }

        private static TClientTransport GetTransport(string[] args)
        {
            var transport = args.FirstOrDefault(x => x.StartsWith("-t"))?.Split(':')?[1];

            Transport selectedTransport;
            if (Enum.TryParse(transport, true, out selectedTransport))
            {
                switch (selectedTransport)
                {
                    case Transport.Tcp:
                        return new TSocketClientTransport(IPAddress.Loopback, 9090);
                    case Transport.NamedPipe:
                        return new TNamedPipeClientTransport(".test");
                    case Transport.Http:
                        return new THttpClientTransport(new Uri("http://localhost:9090"));
                    case Transport.Buffered:
                        return new TBufferedClientTransport(new TSocketClientTransport(IPAddress.Loopback, 9090));
                    case Transport.Framed:
                        throw new NotSupportedException("Framed is not ready for samples");
                    case Transport.TcpTls:
                        throw new NotSupportedException("TcpTls is not ready for samples");
                }
            }

            return new TSocketClientTransport(IPAddress.Loopback, 9090);
        }

        private static TProtocol GetProtocol(string[] args, TClientTransport transport)
        {
            var protocol = args.FirstOrDefault(x => x.StartsWith("-p"))?.Split(':')?[1];

            Protocol selectedProtocol;
            if (Enum.TryParse(protocol, true, out selectedProtocol))
            {
                switch (selectedProtocol)
                {
                    case Protocol.Binary:
                        return new TBinaryProtocol(transport);
                    case Protocol.Compact:
                        return new TCompactProtocol(transport);
                    case Protocol.Json:
                        return new TJsonProtocol(transport);
                }
            }

            return new TBinaryProtocol(transport);
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
                catch (Exception ex)
                {
                    Logger.Error(ex, "Error");
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
