using Arragro.ObjectHistory.QueueProcessorFunction;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace Arragro.ObjectHistory.QueueProcessFunction
{
    public static class Function1
    {
        [FunctionName("ObjectHistoryProcessor")]
        public static void Run([QueueTrigger("objectprocessor", Connection = "")]string myQueueItem, TraceWriter log, ExecutionContext context)
        {
            var configSettings = context.GetObjectHistorySettings();
            var objectHistoryClient = new ObjectHistoryClient(configSettings);
            log.Info($"C# Queue trigger function processed: {myQueueItem}");
            objectHistoryClient.ProcessMessages(myQueueItem).Wait();
        }
    }
}
