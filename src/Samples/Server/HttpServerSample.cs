using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Thrift.Transport;
using Thrift;
using Thrift.Samples;

namespace Server
{
    public class HttpServerSample
    {
        public void Run()
        {
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables(prefix: "ASPNETCORE_")
                .Build();

            var host = new WebHostBuilder()
                .UseConfiguration(config)
                .UseKestrel()
                .UseUrls("http://localhost:9090")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }

        public class Startup
        {
            readonly Logger _logger = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .Enrich.WithThreadId()
                    .WriteTo.ColoredConsole(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm} [{Level}] [ThreadId:{ThreadId}] {SourceContext:l} {Message}{NewLine}{Exception}")
                    .CreateLogger();

            public Startup(IHostingEnvironment env)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(env.ContentRootPath)
                    .AddEnvironmentVariables();

                Configuration = builder.Build();
            }

            public IConfigurationRoot Configuration { get; }

            // This method gets called by the runtime. Use this method to add services to the container.
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddTransient<Calculator.IAsync, CalculatorAsyncHandler>();
                services.AddTransient<TAsyncProcessor, Calculator.AsyncProcessor>();
                services.AddTransient<THttpHandler, THttpHandler>();
            }

            // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
            public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
            {
                loggerFactory.AddSerilog(_logger);
                app.UseMiddleware<THttpHandler>();
            }
        }
    }
}
