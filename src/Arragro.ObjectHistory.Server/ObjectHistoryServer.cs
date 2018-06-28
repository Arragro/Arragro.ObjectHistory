using Arragro.ObjectHistory.Core.Helpers;
using Arragro.ObjectHistory.Core.Models;
using JsonDiffPatchDotNet;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace Arragro.ObjectHistory.Server
{
    public class ObjectHistoryServer
    {
        private const string OBJECT_HISTORY_FILENAME = "objecthistory.json";

        private readonly string _storageConnectionString;

        private readonly CloudStorageAccount _account;
        private readonly CloudBlobClient _blobClient;
        private readonly CloudQueueClient _queueClient;
        private readonly CloudTableClient _tableClient;
        private readonly CloudBlobContainer _objectContainer;
        private readonly CloudQueue _queue;
        private readonly CloudTable _table;
        private readonly CloudTable _globalTable;

        private readonly JsonHelper _jsonHelper;
        private readonly AzureStorageHelper _azureStorageHelper;

        public ObjectHistoryServer(ObjectHistorySettings configurationSettings)
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

            _table = _tableClient.GetTableReference(configurationSettings.ObjectHistoryTable);
            _table.CreateIfNotExistsAsync().Wait();

            _globalTable = _tableClient.GetTableReference(configurationSettings.GlobalHistoryTable);
            _globalTable.CreateIfNotExistsAsync().Wait();


            _azureStorageHelper = new AzureStorageHelper();
            _jsonHelper = new JsonHelper();
        }

        public async Task ProcessMessages()
        {
            var message = GetQueueMessage();

            if (message != null)
            {
                await ValidateAndProcessQueueMessage(message.AsString);
                await _queue.DeleteMessageAsync(message);
            }
        }

        private CloudQueueMessage GetQueueMessage()
        {
            var message = _queue.GetMessageAsync();

            return message.Result;
        }

        private async Task ValidateAndProcessQueueMessage(string blobName)
        {
            var blob = _objectContainer.GetBlockBlobReference(blobName);

            if (!blob.ExistsAsync().Result)
                throw new Exception("Blob file {0} in queue does not exist in either the container.");

            if (blob.Name.EndsWith(".json"))
            {
                var objectHistoryDetailsJson = blob.DownloadTextAsync().Result;
                var objectHistoryDetails = _jsonHelper.GetObjectFromJson<ObjectHistoryDetail>(objectHistoryDetailsJson);

//check id old is the same as last new

                objectHistoryDetails.Diff = ProcessDiff(objectHistoryDetails.OldJson, objectHistoryDetails.NewJson).ToString();

                var objectHistoryJson = _jsonHelper.GetJson(objectHistoryDetails);

                await _azureStorageHelper.UploadJsonFileAsync(_objectContainer, objectHistoryDetails.Folder, "ObjectHistory.json", objectHistoryJson);

                await _azureStorageHelper.AddObjectHistoryEntityRecord(objectHistoryDetails, _table);

                await _azureStorageHelper.AddObjectHistoryGlobal(objectHistoryDetails, _globalTable);

                await blob.DeleteAsync();
            }
        }

        private JToken ProcessDiff(string oldjson, string newjson)
        {
            try
            {
                var jdp = new JsonDiffPatch();
                var left = JToken.Parse(oldjson);
                var right = JToken.Parse(newjson);

                JToken patch = jdp.Diff(left, right);

                return patch;
            }
            catch (Exception ex)
            {

                throw new Exception(String.Format("Something went wrong porcessing the json diff. Please review the exception. {0}", ex));
            }

        }

    }
}
