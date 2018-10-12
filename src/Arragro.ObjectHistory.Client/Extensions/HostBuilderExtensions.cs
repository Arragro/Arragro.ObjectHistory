using Arragro.ObjectHistory.Core;
using Arragro.ObjectHistory.Core.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Arragro.ObjectHistory.Client.Extensions
{
    public static class ServicesConfiguration
    {
        public static IServiceCollection AddArragroObjectHistoryClient<TObjectLogsSecurityAttribute>(
            this IServiceCollection services, 
            IConfiguration configuration)
            where TObjectLogsSecurityAttribute : class, IObjectLogsSecurityAttribute
        {
            var objectHistorySettings = new ObjectHistoryClientSettings();
            configuration.GetSection("ObjectHistoryClientSettings").Bind(objectHistorySettings);
            return services
                .AddTransient<IObjectLogsSecurityAttribute, TObjectLogsSecurityAttribute>()
                .AddSingleton(objectHistorySettings)
                .AddSingleton(new ObjectHistoryService(objectHistorySettings.AzureStorageConnectionString))
                .AddSingleton<ObjectHistoryClient>();
        }
    }
}
