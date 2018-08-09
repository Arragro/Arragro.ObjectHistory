using Arragro.ObjectHistory.Core.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Arragro.ObjectHistory.Core.Helpers
{
    public class ObjectHistoryService
    {
        public readonly string ObjectHistoryRequestFileName; 
        public readonly string ObjectHistoryFileName;
        public readonly string StorageConnectionString;
        public readonly string ApplicationName;

        public CloudStorageAccount Account { get; }
        public CloudBlobClient BlobClient;
        public CloudQueueClient QueueClient;
        public CloudTableClient TableClient;
        public CloudBlobContainer ObjectContainer;
        public CloudQueue Queue;
        public CloudTable Table;
        public CloudTable GlobalTable;

        public JsonHelper JsonHelper;
        public AzureStorageHelper AzureStorageHelper;

        public ObjectHistoryService(ObjectHistorySettings configurationSettings)
        {
            ApplicationName = configurationSettings.ApplicationName;
            StorageConnectionString = configurationSettings.StorageConnectionString;
            ObjectHistoryRequestFileName = "objecthistoryrequest.json";
            ObjectHistoryRequestFileName = "ObjectHistory.json";


            Account = CloudStorageAccount.Parse(StorageConnectionString);
            BlobClient = Account.CreateCloudBlobClient();
            QueueClient = Account.CreateCloudQueueClient();
            TableClient = Account.CreateCloudTableClient();

            Queue = QueueClient.GetQueueReference(configurationSettings.MessageQueueName);
            Queue.CreateIfNotExistsAsync().Wait();

            ObjectContainer = BlobClient.GetContainerReference(configurationSettings.ObjectContainerName);
            ObjectContainer.CreateIfNotExistsAsync().Wait();

            Table = TableClient.GetTableReference(configurationSettings.ObjectHistoryTable);
            Table.CreateIfNotExistsAsync().Wait();

            GlobalTable = TableClient.GetTableReference(configurationSettings.GlobalHistoryTable);
            GlobalTable.CreateIfNotExistsAsync().Wait();

            AzureStorageHelper = new AzureStorageHelper();
            JsonHelper = new JsonHelper();
        }
    }
}
