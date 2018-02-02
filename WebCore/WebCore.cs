using System.Collections.Generic;
using System.Fabric;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Serilog;

namespace WebCore
{
    internal sealed class WebCore : StatelessService
    {
        public WebCore(StatelessServiceContext context) : base(context) { }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[]
            {
                new ServiceInstanceListener(serviceContext =>
                    new WebListenerCommunicationListener(serviceContext, "APIServiceEndpointHttp",
                        (url, listener) =>
                        {
                            return ConfigureWebHost(serviceContext, url, listener)
                                .ConfigureAppConfiguration((hostingContext, config) =>
                                {
                                    //var configuration = config
                                    //    .AddJsonFile("appsettings.json", true, true)
                                    //    .AddEnvironmentVariables()
                                    //    .Build();

                                    //Log.Logger = new LoggerConfiguration()
                                    //    .ReadFrom
                                    //    .Configuration(configuration)
                                    //    .CreateLogger();
                                })
                                .ConfigureLogging((hostingContext, logging) =>
                                {
                                    //logging.AddSerilog(dispose: true);
                                    logging.AddConsole();
                                    logging.AddDebug();
                                })
                                .Build();
                        }))
            };
        }

        private static IWebHostBuilder ConfigureWebHost(StatelessServiceContext serviceContext, string url, AspNetCoreCommunicationListener listener)
        {
            return new WebHostBuilder()
                .UseHttpSys()
                .ConfigureServices(services => services.AddSingleton(serviceContext))
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                .UseUrls(url);
        }
    }
}