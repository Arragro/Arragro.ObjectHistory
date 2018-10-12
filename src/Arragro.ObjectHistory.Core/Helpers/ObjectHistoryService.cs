using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;

namespace Arragro.ObjectHistory.Core.Helpers
{
    public class ObjectHistoryService
    {
        public const string ObjectHistoryQueueName = "objectprocessor";
        public const string ObjectContainerName = "objectprocessor";
        public const string ObjectHistoryRequestFileName = "objecthistoryrequest.json";
        public const string ObjectHistoryFileName = "ObjectHistory.json";
        public readonly string AzureStorageConnectionString;

        public CloudStorageAccount Account { get; }
        public CloudBlobClient BlobClient;
        public CloudQueueClient QueueClient;
        public CloudTableClient TableClient;
        public CloudBlobContainer ObjectContainer;
        public CloudQueue Queue;

        public JsonHelper JsonHelper;
        public AzureStorageHelper AzureStorageHelper;

        public ObjectHistoryService(string azureStorageConnectionString)
        {
            AzureStorageConnectionString = azureStorageConnectionString;

            Account = CloudStorageAccount.Parse(AzureStorageConnectionString);
            BlobClient = Account.CreateCloudBlobClient();
            QueueClient = Account.CreateCloudQueueClient();
            TableClient = Account.CreateCloudTableClient();

            Queue = QueueClient.GetQueueReference(ObjectHistoryQueueName);
            Queue.CreateIfNotExistsAsync().Wait();

            ObjectContainer = BlobClient.GetContainerReference(ObjectContainerName);
            ObjectContainer.CreateIfNotExistsAsync().Wait();

            AzureStorageHelper = new AzureStorageHelper();
            JsonHelper = new JsonHelper();
        }

        public async Task<CloudTable> GetObjectHistoryTableAsync(string historyTableName)
        {
            var table = TableClient.GetTableReference(historyTableName);
            await table.CreateIfNotExistsAsync();
            return table;
        }

        public async Task<CloudTable> GetGlobalHistoryTableAsync(string globalHistoryTableName)
        {
            var table = TableClient.GetTableReference(globalHistoryTableName);
            await table.CreateIfNotExistsAsync();
            return table;
        }

        public async Task<CloudBlobContainer> GetObjectHistoryContainerAsync(string containerName)
        {
            var container = BlobClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();
            return container;
        }
    }
}
