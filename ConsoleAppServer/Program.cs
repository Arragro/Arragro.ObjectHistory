using System;

namespace ConsoleAppServer
{
    class Program
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

                        services.AddArragroObjectHistory(objectHistoryClientSettings);
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
}
