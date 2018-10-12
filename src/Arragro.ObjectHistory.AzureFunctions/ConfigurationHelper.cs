using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;

namespace Arragro.ObjectHistory.AzureFunctions
{
    public static class ConfigurationHelper
    {
        private static object _locker = new object();
        private static Settings _settings = null;

        private static Settings GetConfiguration(this ExecutionContext context)
        {
            if (_settings == null)
            {
                lock (_locker)
                {
                    if (_settings == null)
                    {
                        var configurationBuilder = new ConfigurationBuilder()
                           .SetBasePath(context.FunctionAppDirectory)
                           .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                           .AddEnvironmentVariables()
                           .Build();
                        _settings = new Settings
                        {
                            AzureWebJobsStorage = configurationBuilder["Values:AzureWebJobsStorage"]
                        };
                    }
                }
            }
            return _settings;
        }

        public static Settings GetObjectHistorySettings(this ExecutionContext context)
        {
            return context.GetConfiguration();
        }
    }
}
