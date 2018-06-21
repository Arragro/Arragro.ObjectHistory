using System.Threading.Tasks;
using Arragro.ObjectHistory.Core.Models;
using Arragro.ObjectHistory.ObjectHistoryClientProvider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
                    services.AddOptions()
                    //services.Configure<ObjectHistoryClientConfiguration>(hostContext.Configuration.GetSection("ObjectHistoryClientSettings"));
                    .AddSingleton(hostContext.Configuration.Get<ConfigurationSettings>())
                    .AddSingleton<ObjectHistoryClient>();

                    services.AddSingleton<IHostedService, App>();
                })
                .ConfigureLogging((hostingContext, logging) => {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                });

            await builder.RunConsoleAsync();
        }
    }
}
