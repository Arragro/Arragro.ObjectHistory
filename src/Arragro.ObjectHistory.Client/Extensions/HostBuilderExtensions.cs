using Arragro.ObjectHistory.Core;
using Arragro.ObjectHistory.Core.Helpers;
using Arragro.ObjectHistory.Core.Interfaces;
using Arragro.ObjectHistory.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Arragro.ObjectHistory.Client.Extensions
{
    public static class ServicesConfiguration
    {
        public static IServiceCollection AddArragroObjectHistoryClient<TObjectLogsSecurityAttribute>(
            this IServiceCollection services,
            ObjectHistorySettings objectHistorySettings)
            where TObjectLogsSecurityAttribute : class, IObjectLogsSecurityAttribute
        {
            services
                .AddTransient<IObjectLogsSecurityAttribute, TObjectLogsSecurityAttribute>()
                .AddSingleton(objectHistorySettings)
                .AddSingleton<IStorageHelper, AzureStorageHelper>()
                .AddSingleton<IObjectHistoryClient, ObjectHistoryClient>();

            return services;
        }
    }
}
