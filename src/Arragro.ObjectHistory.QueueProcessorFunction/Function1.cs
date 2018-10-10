using Arragro.ObjectHistory.QueueProcessorFunction;
using Arragro.ObjectHistory.Core.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace Arragro.ObjectHistory.QueueProcessFunction
{
    public static class Function1
    {
        [FunctionName("ObjectHistoryProcessor")]
        public static void Run([QueueTrigger("objectprocessor", Connection = "")] ObjectHistoryMessge myQueueItem, TraceWriter log, ExecutionContext context)
        {
            var configSettings = context.GetObjectHistorySettings();
            var objectHistoryProcessor = new ObjectHistoryProcessor(configSettings);
            log.Info($"C# Queue trigger function processed: {myQueueItem}");
            objectHistoryProcessor.ProcessMessages(myQueueItem.Message).Wait();
        }
    }
}
