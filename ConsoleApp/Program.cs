using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Arragro.ObjectHistory.Client.Extensions;
using Arragro.ObjectHistory.Server.Extensions;
using Arragro.ObjectHistory.Core.Models;

namespace ConsoleApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true);
                    config.AddEnvironmentVariables();

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var objectHistoryClientSettings = new ObjectHistorySettings();
                    hostContext.Configuration.GetSection("ObjectHistoryClientSettings").Bind(objectHistoryClientSettings);
                    services.AddArragroObjectHistoryServer(objectHistoryClientSettings);
                    services.AddArragroObjectHistoryClient(objectHistoryClientSettings);
                    services.AddSingleton<IHostedService, App>();
                })
                .ConfigureLogging((hostingContext, logging) => {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                });

            var host = new HostBuilder()
                .Build();

            host.RunAsync();

            await builder.RunConsoleAsync();
        }
    }
}
