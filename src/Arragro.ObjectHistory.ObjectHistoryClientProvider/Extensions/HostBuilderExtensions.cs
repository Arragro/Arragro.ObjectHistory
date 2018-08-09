using Arragro.ObjectHistory.Core.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Arragro.ObjectHistory.Client.Extensions
{
    public static class ServicesConfiguration
    {
        public static IServiceCollection AddArragroObjectHistoryClient(this IServiceCollection services, ObjectHistorySettings configuration)
        {
            return services
                .AddSingleton(configuration)
                .AddSingleton<ObjectHistoryClient>();
        }
    }
}
