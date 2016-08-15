//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using Thrift.Transport;

// TODO: Asp.net core sample

//namespace Server
//{
//    public class HttpServerSample
//    {
//        public void Run()
//        {
//            var config = new ConfigurationBuilder()
//                .AddEnvironmentVariables(prefix: "ASPNETCORE_")
//                .Build();

//            var host = new WebHostBuilder()
//                .UseConfiguration(config)
//                .UseKestrel()
//                .UseContentRoot(Directory.GetCurrentDirectory())
//                .UseIISIntegration()
//                .UseStartup<Startup>()
//                .Build();

//            host.Run();
//        }

//        public class Startup
//        {
//            public Startup(IHostingEnvironment env)
//            {
//                var builder = new ConfigurationBuilder()
//                    .SetBasePath(env.ContentRootPath)
//                    //.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
//                    //.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
//                    .AddEnvironmentVariables();
//                Configuration = builder.Build();
//            }

//            public IConfigurationRoot Configuration { get; }

//            // This method gets called by the runtime. Use this method to add services to the container.
//            public void ConfigureServices(IServiceCollection services)
//            {
//                // Add framework services.
//                //services.AddMvc();
//            }

//            // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
//            public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
//            {
//                loggerFactory.AddConsole().AddDebug();
//                app.UseMiddleware<THttpHandler>();
//                app.UseMvc();
//            }
//        }
//    }
//}
