using Arragro.Core.HostedServices;
using Arragro.ObjectHistory.Core.Helpers;
using Arragro.ObjectHistory.Core.Interfaces;
using Arragro.ObjectHistory.Core.Models;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace Arragro.ObjectHistory.HostedService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            var objectHistorySettings = new ObjectHistorySettings();
            Configuration.GetSection("ObjectHistorySettings").Bind(objectHistorySettings);

            services.AddSingleton(objectHistorySettings)
                .AddSingleton<IStorageHelper, AzureStorageHelper>();
            services.AddCustomHealthCheck(Configuration, objectHistorySettings)
                .AddOptions()
                .AddScoped<ObjectHistoryProcessor>()
                .AddQueueJob<ObjectProcessorHostedService>(
                    objectHistorySettings.AzureStorageConnectionString,
                    objectHistorySettings.ObjectQueueName
                 );

            //var tempTelemetaryPath = $"{Directory.GetCurrentDirectory()}/App_Data/app-insights";
            //if (!Directory.Exists(tempTelemetaryPath))
            //    Directory.CreateDirectory(tempTelemetaryPath);

            //services.AddSingleton(typeof(ITelemetryChannel),
            //                    new ServerTelemetryChannel() { StorageFolder = tempTelemetaryPath });

            //services
            //        .AddApplicationInsightsTelemetryWorkerService(Configuration.GetValue<string>("ApplicationInsights:InstrumentationKey"));
        }


        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
                    ResultStatusCodes =
                    {
                        [HealthStatus.Healthy] = StatusCodes.Status200OK,
                        [HealthStatus.Degraded] = StatusCodes.Status200OK,
                        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                    }
                });
                endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
                {
                    Predicate = r => r.Name.Contains("self")
                });
            });
        }
    }
}
