using Arragro.ObjectHistory.Core.Helpers;
using Arragro.ObjectHistory.Core.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Arragro.ObjectHistory.Client
{
    public class ObjectHistoryClient
    {
        private const string OBJECT_HISTORY_FILENAME = "objecthistoryrequest.json";

        private readonly string _storageConnectionString;

        private readonly CloudStorageAccount _account;
        private readonly CloudBlobClient _blobClient;
        private readonly CloudQueueClient _queueClient;
        private readonly CloudTableClient _tableClient;
        private readonly CloudBlobContainer _objectContainer;
        private readonly CloudQueue _queue;
        private readonly CloudTable _table;

        private readonly JsonHelper _jsonHelper;
        private readonly AzureStorageHelper _azureStorageHelper;

        public ObjectHistoryClient(ObjectHistorySettings configurationSettings)
        {
            _storageConnectionString = configurationSettings.StorageConnectionString;
            _account = CloudStorageAccount.Parse(_storageConnectionString);
            _blobClient = _account.CreateCloudBlobClient();
            _queueClient = _account.CreateCloudQueueClient();
            _tableClient = _account.CreateCloudTableClient();

            _queue = _queueClient.GetQueueReference(configurationSettings.MessageQueueName);
            _queue.CreateIfNotExistsAsync().Wait();

            _objectContainer = _blobClient.GetContainerReference(configurationSettings.ObjectContainerName);
            _objectContainer.CreateIfNotExistsAsync().Wait();

            _table = _tableClient.GetTableReference(configurationSettings.TableName);
            _table.CreateIfNotExistsAsync().Wait();

            _azureStorageHelper = new AzureStorageHelper();
            _jsonHelper = new JsonHelper();

        }

        public async Task SaveObjectHistoryAsync<T>(Func<string> getKeys, T oldObject, T newObject, string user)
        {
            var fullyQualifiedName = typeof(T).FullName;
            var key = getKeys();
            var partitionKey = $"{fullyQualifiedName}-{key}";

            var trackedObject = new ObjectHistoryDetails(partitionKey,
                                            string.Format("{0:D19}", DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks),
                                            DateTime.UtcNow.ToString(), user, Guid.NewGuid().ToString())
            {
                OldJson = _jsonHelper.GetJson(oldObject, true),
                NewJson = _jsonHelper.GetJson(newObject, true)
            };

            var trackedObjectJson = _jsonHelper.GetJson(trackedObject);
            
            await _azureStorageHelper.UploadJsonFileAsync(_objectContainer, trackedObject.Folder, OBJECT_HISTORY_FILENAME, trackedObjectJson);
            
            await _azureStorageHelper.SendQueueMessage(_queue, String.Format("{0}/{1}",trackedObject.Folder, OBJECT_HISTORY_FILENAME));
        }

        

        public async Task<string> GetObjectHistoryAsync<T>(Func<string> getKeys, string continuationToken)
        {
            var fullyQualifiedName = typeof(T).FullName;
            var key = getKeys();
            var partitionKey = $"{fullyQualifiedName}-{key}";

            var entities = _jsonHelper.GetJson( await _azureStorageHelper.GetObjectHistoryRecordsByPartitionKey(partitionKey, _table, null));

            return entities;
        }
    }
}
