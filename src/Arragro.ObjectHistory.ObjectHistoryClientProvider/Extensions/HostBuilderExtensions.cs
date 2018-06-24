using Arragro.ObjectHistory.Core.Models;
using Arragro.ObjectHistory.ObjectHistoryClientProvider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Arragro.ObjectHistory.ObjectHistoryClientProvider.Extensions
{
    public static class ServicesConfiguration
    {
        public static IServiceCollection AddArragroObjectHistory(this IServiceCollection services, ObjectHistoryClientSettings configuration)
        {
            return services.AddOptions()
                .AddSingleton(configuration)
                .AddSingleton<ObjectHistoryClient>();
        }
    }
}
