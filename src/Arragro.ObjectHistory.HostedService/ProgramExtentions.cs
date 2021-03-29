using Arragro.ObjectHistory.Core;
using Arragro.ObjectHistory.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Arragro.ObjectHistory.HostedService
{
    public static class ProgramExtentions
    {
        public static IServiceCollection AddCustomHealthCheck(
            this IServiceCollection services, 
            IConfiguration configuration,
            ObjectHistorySettings objectHistorySettings)
        {
            var hcBuilder = services.AddHealthChecks();

            hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

            var storageConnection = configuration.GetValue<string>("ConnectionStrings:StorageConnection");

            hcBuilder.AddAzureBlobStorage(storageConnection, objectHistorySettings.ObjectInputContainerName, name: "Input");
            hcBuilder.AddAzureBlobStorage(storageConnection, objectHistorySettings.ObjectOutputContainerName, name: "Output");
            hcBuilder.AddAzureQueueStorage(storageConnection, objectHistorySettings.ObjectQueueName);
            hcBuilder.AddAzureTable(storageConnection, objectHistorySettings.GlobalHistoryTable, name: "GlobalHistoryTable");
            hcBuilder.AddAzureTable(storageConnection, objectHistorySettings.ObjectHistoryTable, name: "ObjectHistoryTable");

            return services;
        }

        public static ILoggingBuilder UseSerilog(this ILoggingBuilder builder, IConfiguration configuration)
        {
            //var seqServerUrl = configuration["Serilog:SeqServerUrl"];
            //var logstashUrl = configuration["Serilog:LogstashgUrl"];

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.WithProperty("ApplicationContext", Program.AppName)
                .Enrich.FromLogContext()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                //.WriteTo.Seq(string.IsNullOrWhiteSpace(seqServerUrl) ? "http://seq" : seqServerUrl)
                //.WriteTo.Http(string.IsNullOrWhiteSpace(logstashUrl) ? "http://logstash:8080" : logstashUrl)
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            return builder;
        }
    }
}
