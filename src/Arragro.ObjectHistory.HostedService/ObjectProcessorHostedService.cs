using Arragro.Core.HostedServices;
using Arragro.ObjectHistory.Core.Models;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Arragro.ObjectHistory.HostedService
{
    public class ObjectProcessorHostedService : QueueJobService
    {
        private readonly IServiceProvider _serviceProvider;

        public ObjectProcessorHostedService(
            IQueueConfig<ObjectProcessorHostedService> config,
            ILogger<ObjectProcessorHostedService> logger,
            IServiceProvider serviceProvider)
            : base(config.QueueName, config.ConnectionString, config.CronExpression, config.IncludeSeconds, config.TimeZoneInfo, logger)
        {
            _serviceProvider = serviceProvider;
        }

        public override async Task DoWork(QueueMessage message, CancellationToken cancellationToken)
        {
            var objectHistoryMessge = JsonConvert.DeserializeObject<ObjectHistoryMessge>(message.MessageText);
            using (var scope = _serviceProvider.CreateScope())
            {
                var objectHistoryProcessor = scope.ServiceProvider.GetRequiredService<ObjectHistoryProcessor>();
                await objectHistoryProcessor.ProcessMessagesAsync(objectHistoryMessge.Message);
            }
        }
    }
}
