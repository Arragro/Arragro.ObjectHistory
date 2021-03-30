using Arragro.ObjectHistory.Core.Helpers;
using Arragro.ObjectHistory.Core.Models;
using Azure.Storage.Queues;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Arragro.ObjectHistory.IntegrationTests
{
    public class Utils
    {
        public static async Task ProcessQueue(QueueClient queueClient, ObjectHistoryProcessor objectHistoryProcessor)
        {
            do
            {
                var queueMessage = await queueClient.ReceiveMessageAsync();
                if (queueMessage.Value == null)
                    break;
                var objectHistoryMessge = JsonConvert.DeserializeObject<ObjectHistoryMessge>(queueMessage.Value.MessageText);
                await objectHistoryProcessor.ProcessQueueMessageAsync(objectHistoryMessge.Message);
                await queueClient.DeleteMessageAsync(queueMessage.Value.MessageId, queueMessage.Value.PopReceipt);
            } while (true);
        }
    }
}
