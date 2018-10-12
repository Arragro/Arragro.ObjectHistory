using Arragro.ObjectHistory.Core.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Arragro.ObjectHistory.AzureFunctions
{
    public static class ObjectHistoryProcessorFunction
    {
        [FunctionName("ObjectHistoryProcessorFunction")]
        public static void Run([QueueTrigger("objectprocessor", Connection = "")] ObjectHistoryMessge myQueueItem, ILogger log, ExecutionContext context)
        {
            var configSettings = context.GetObjectHistorySettings();
            var objectHistoryProcessor = new ObjectHistoryProcessor(configSettings);
            objectHistoryProcessor.ProcessMessages(myQueueItem.Message).Wait();
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem.Message}");
        }
    }
}
