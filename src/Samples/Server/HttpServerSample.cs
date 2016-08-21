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
                services.AddTransient<Calculator.IAsync, CalculatorHandler>();
                services.AddTransient<TAsyncProcessor, Calculator.AsyncProcessor>();
                services.AddTransient<THttpHandler, THttpHandler>();
            }

            // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
            public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
            {
                loggerFactory.AddConsole().AddDebug();
                app.UseMiddleware<THttpHandler>();
            }
        }

        public class CalculatorHandler : Calculator.IAsync
        {
            Dictionary<int, SharedStruct> _log;

            public CalculatorHandler()
            {
                _log = new Dictionary<int, SharedStruct>();
            }

            public async Task<SharedStruct> GetStructAsync(int key)
            {
                Console.WriteLine("GetStructAsync({0})", key);
                return await Task.FromResult(_log[key]);
            }

            public async Task PingAsync()
            {
                Console.WriteLine("PingAsync()");
                await Task.Delay(10, CancellationToken.None);
            }

            public async Task<int> AddAsync(int num1, int num2)
            {
                Console.WriteLine("AddAsync({0},{1})", num1, num2);
                return await Task.FromResult(Add(num1, num2));
            }

            public async Task<int> CalculateAsync(int logid, Work w)
            {
                Console.WriteLine("CalculateAsync({0}, [{1},{2},{3}])", logid, w.Op, w.Num1, w.Num2);
                return await Task.FromResult(Calculate(logid, w));
            }

            public async Task ZipAsync()
            {
                Console.WriteLine("ZipAsync()");
                await Task.Delay(10, CancellationToken.None);
            }

            public void Ping()
            {
                Console.WriteLine("Ping()");
            }

            public int Add(int num1, int num2)
            {
                Console.WriteLine("Add({0},{1})", num1, num2);
                return num1 + num2;
            }

            public void Zip()
            {
                Console.WriteLine("Zip()");
            }

            public int Calculate(int logid, Work w)
            {
                Console.WriteLine("Calculate({0}, [{1},{2},{3}])", logid, w.Op, w.Num1, w.Num2);
                var val = 0;

                switch (w.Op)
                {
                    case Operation.Add:
                        val = w.Num1 + w.Num2;
                        break;

                    case Operation.Substract:
                        val = w.Num1 - w.Num2;
                        break;

                    case Operation.Multiply:
                        val = w.Num1 * w.Num2;
                        break;

                    case Operation.Divide:
                        if (w.Num2 == 0)
                        {
                            var io = new InvalidOperation
                            {
                                WhatOp = (int)w.Op,
                                Why = "Cannot divide by 0"
                            };

                            throw io;
                        }
                        val = w.Num1 / w.Num2;
                        break;

                    default:
                        {
                            var io = new InvalidOperation
                            {
                                WhatOp = (int)w.Op,
                                Why = "Unknown operation"
                            };

                            throw io;
                        }
                }

                var entry = new SharedStruct
                {
                    Key = logid,
                    Value = val.ToString()
                };

                _log[logid] = entry;

                return val;
            }
        }
    }
}
