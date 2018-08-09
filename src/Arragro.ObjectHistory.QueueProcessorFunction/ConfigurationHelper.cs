using Arragro.ObjectHistory.Core.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;

namespace Arragro.ObjectHistory.QueueProcessFunction
{
    public static class ConfigurationHelper
    {
        private static object _locker = new object();
        private static ObjectHistorySettings _objectHistorySettings = null;

        private static ObjectHistorySettings GetConfiguration(this ExecutionContext context)
        {
            if (_objectHistorySettings == null)
            {
                lock (_locker)
                {
                    if (_objectHistorySettings == null)
                    {
                        var configurationBuilder = new ConfigurationBuilder()
                           .SetBasePath(context.FunctionAppDirectory)
                           .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                           .AddEnvironmentVariables()
                           .Build();
                        _objectHistorySettings = new ObjectHistorySettings
                        {
                            ApplicationName = configurationBuilder["ObjectHistorySettings:ApplicationName"],
                            StorageConnectionString = configurationBuilder["ObjectHistorySettings:StorageConnectionString"],
                            ObjectContainerName = configurationBuilder["ObjectHistorySettings:ObjectContainerName"],
                            MessageQueueName = configurationBuilder["ObjectHistorySettings:MessageQueueName"],
                            ObjectHistoryTable = configurationBuilder["ObjectHistorySettings:ObjectHistoryTable"],
                            GlobalHistoryTable = configurationBuilder["ObjectHistorySettings:GlobalHistoryTable"],
                        };
                    }
                }
            }
            return _objectHistorySettings;
        }

        public static ObjectHistorySettings GetObjectHistorySettings(this ExecutionContext context)
        {
            return context.GetConfiguration();
        }
    }
}
