using Arragro.ObjectHistory.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Arragro.ObjectHistory.Server.Extensions
{
    public static class ServicesConfiguration
    {
        public static IServiceCollection AddArragroObjectHistoryServer(this IServiceCollection services, ObjectHistorySettings configuration)
        {
            return services.AddOptions()
                .AddSingleton(configuration)
                .AddSingleton<ObjectHistoryServer>();
        }
    }
}
