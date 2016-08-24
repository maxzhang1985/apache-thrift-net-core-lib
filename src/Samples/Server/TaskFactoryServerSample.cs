using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Thrift.Samples;
using Thrift.Server;
using Thrift.Transport;

namespace Server
{
    public class TaskFactoryServerSample
    {
        readonly Logger _logger = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .Enrich.WithThreadId()
                    .WriteTo.ColoredConsole(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm} [{Level}] [ThreadId:{ThreadId}] {SourceContext:l} {Message}{NewLine}{Exception}")
                    .CreateLogger();
        
        public void Run()
        {
            _logger.Verbose("Selected TAsyncServer with TCP transport");

            try
            {
                var source = new CancellationTokenSource();
                var fabric = new LoggerFactory().AddSerilog(_logger);

                var handler = new CalculatorAsyncHandler();
                var processor = new Calculator.AsyncProcessor(handler);
                var serverTransport = new TServerSocket(9090);
                var server = new TAsyncServer(processor, serverTransport, fabric);

                _logger.Verbose("Starting the server...");
                server.ServeAsync(source.Token).GetAwaiter().GetResult();

                _logger.Verbose("Press any key to stop...");
                Console.ReadLine();
                source.Cancel();
            }
            catch (Exception x)
            {
                _logger.Error(x.ToString());
            }

            _logger.Verbose("Server stopped.");
        }
    }
}
