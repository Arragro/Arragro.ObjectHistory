using Arragro.ObjectHistory.Core.Models;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using HealthChecks.Azure.Data.Tables;
using HealthChecks.Azure.Storage.Blobs;
using HealthChecks.Azure.Storage.Queues;
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

            hcBuilder.Services.AddSingleton(sp => new BlobServiceClient(storageConnection));
            hcBuilder.Services.AddSingleton(sp => new QueueServiceClient(storageConnection));
            hcBuilder.Services.AddSingleton(sp => new TableServiceClient(storageConnection));
            hcBuilder.AddAzureBlobStorage(optionsFactory: sp => new AzureBlobStorageHealthCheckOptions()
            {
                ContainerName = objectHistorySettings.ObjectInputContainerName
            }, name: "Input");
            hcBuilder.AddAzureBlobStorage(optionsFactory: sp => new AzureBlobStorageHealthCheckOptions()
            {
                ContainerName = objectHistorySettings.ObjectOutputContainerName
            }, name: "Output");

            hcBuilder.AddAzureQueueStorage(optionsFactory: sp => new AzureQueueStorageHealthCheckOptions()
            {
                QueueName = objectHistorySettings.ObjectQueueName
            });

            hcBuilder.AddAzureTable(optionsFactory: sp => new AzureTableServiceHealthCheckOptions()
            {
                TableName = objectHistorySettings.GlobalHistoryTable
            }, name: "GlobalHistoryTable");
            hcBuilder.AddAzureTable(optionsFactory: sp => new AzureTableServiceHealthCheckOptions()
            {
                TableName = objectHistorySettings.ObjectHistoryTable
            }, name: "ObjectHistoryTable");

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
