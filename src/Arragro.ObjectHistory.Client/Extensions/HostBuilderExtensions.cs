using Arragro.Core.EntityFrameworkCore.Extensions;
using Arragro.ObjectHistory.AzureStorage;
using Arragro.ObjectHistory.Core;
using Arragro.ObjectHistory.Core.Helpers;
using Arragro.ObjectHistory.Core.Interfaces;
using Arragro.ObjectHistory.Core.Models;
using Arragro.ObjectHistory.EFCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Reflection;

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
                .AddScoped<IObjectLogsSecurityAttribute, TObjectLogsSecurityAttribute>()
                .AddSingleton(objectHistorySettings)                
                .AddScoped<IObjectHistoryClient, ObjectHistoryClient>()
                .AddScoped<ObjectHistoryProcessor>();

            var sqlliteInMemoryConnectionString = "DataSource=:memory:";

            switch (objectHistorySettings.StorageType)
            {
                case StorageType.Postgres:
                    services.AddDbContext<ArragroObjectHistoryPGContext>(
                        options => options.UseNpgsql(objectHistorySettings.DatabaseConnectionString)
                    );
                    services.AddScoped<ArragroObjectHistoryBaseContext, ArragroObjectHistoryPGContext>();
                    services.AddScoped<IStorageHelper, EFStorageHelper>();
                    break;
                case StorageType.SqlServer:
                    services.AddDbContext<ArragroObjectHistoryContext>(
                        options => options.UseSqlServer(objectHistorySettings.DatabaseConnectionString)
                    );
                    services.AddScoped<ArragroObjectHistoryBaseContext, ArragroObjectHistoryContext>();
                    services.AddScoped<IStorageHelper, EFStorageHelper>();
                    break;
                case StorageType.Sqlite:
                    if (objectHistorySettings.DatabaseConnectionString != sqlliteInMemoryConnectionString)
                    {
                        var builder = new SqliteConnectionStringBuilder(objectHistorySettings.DatabaseConnectionString);
                        builder.DataSource = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), builder.DataSource));
                        Directory.CreateDirectory(Path.GetDirectoryName(builder.DataSource));
                        sqlliteInMemoryConnectionString = builder.ToString();
                    }

                    services.AddDbContext<ArragroObjectHistorySqliteContext>(
                        options => options.UseSqlite(sqlliteInMemoryConnectionString)
                    );
                    services.AddScoped<ArragroObjectHistoryBaseContext, ArragroObjectHistorySqliteContext>();
                    services.AddScoped<IStorageHelper, EFStorageHelper>();
                    break;
                default:
                    services.AddScoped<IStorageHelper, AzureStorageHelper>();
                    break;
            }

            if (objectHistorySettings.StorageType != StorageType.AzureStorage)
            {
                var serviceProvider = services.BuildServiceProvider();

                var arragroObjectHistoryContext = serviceProvider.GetRequiredService<ArragroObjectHistoryBaseContext>();

                if (objectHistorySettings.DatabaseConnectionString != sqlliteInMemoryConnectionString &&
                    (!arragroObjectHistoryContext.Exists() || (!arragroObjectHistoryContext.AllMigrationsApplied())))
                    arragroObjectHistoryContext.Database.Migrate();
            }

            return services;
        }
    }
}
