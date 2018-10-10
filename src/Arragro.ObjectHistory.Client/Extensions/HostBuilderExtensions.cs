using Arragro.ObjectHistory.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration.Binder;

namespace Arragro.ObjectHistory.Client.Extensions
{
    public static class ServicesConfiguration
    {
        public static IServiceCollection AddArragroObjectHistoryClient(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            var objectHistorySettings = new ObjectHistorySettings();
            configuration.GetSection("ObjectHistoryClientSettings").Bind(objectHistorySettings);
            return services
                .AddSingleton(objectHistorySettings)
                .AddSingleton<ObjectHistoryClient>();
        }
    }
}
