
using Arragro.ObjectHistory.Core.Extentions;
using Arragro.ObjectHistory.Core.Helpers;
using Arragro.ObjectHistory.Core.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Queues.Models;
using JsonDiffPatchDotNet;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace Arragro.ObjectHistory.AzureFunctions
{
    public class ObjectHistoryProcessor
    {
        private readonly ObjectHistoryService _objectHistoryService;

        public ObjectHistoryProcessor(Settings settings)
        {
            _objectHistoryService = new ObjectHistoryService(settings.AzureWebJobsStorage);
        }

        public async Task ProcessMessages(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException("message");

            await ValidateAndProcessQueueMessage(message);
        }

        private async Task<QueueMessage> GetQueueMessageAsync()
        {
            return await _objectHistoryService.QueueClient.ReceiveMessageAsync();
        }

        private async Task<BlobClient> GetObjectHistoryBlobAsync(BlobContainerClient blobContainerClient, string blobName)
        {
            try
            {
                var blobClient = blobContainerClient.GetBlobClient(blobName);

                if (!(await blobClient.ExistsAsync()))
                    throw new Exception("Blob file {0} in queue does not exist in the container.");

                if (!blobClient.Name.EndsWith(".json"))
                    throw new Exception("Blob file extension for {0} is not .json ");

                return blobClient;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task ValidateAndProcessQueueMessage(string blobName)
        {
            var blob = await GetObjectHistoryBlobAsync(_objectHistoryService.BlobContainerClient, blobName);

            var objectHistoryDetailsJson = await blob.DownloadTextAsync();
            var objectHistoryDetails = _objectHistoryService.JsonHelper.GetObjectFromJson<ObjectHistoryDetailRead>(objectHistoryDetailsJson);

            var objectHistoryTable = await _objectHistoryService.GetObjectHistoryTableAsync(objectHistoryDetails.ObjectHistorySettings.ObjectHistoryTable);
            var globalHistoryTable = await _objectHistoryService.GetObjectHistoryTableAsync(objectHistoryDetails.ObjectHistorySettings.GlobalHistoryTable);
            var objectHistoryContainer = await _objectHistoryService.GetObjectHistoryContainerAsync(objectHistoryDetails.ObjectHistorySettings.ObjectContainerName);

            if (!objectHistoryDetails.IsAdd)
            {
                objectHistoryDetails.Diff = ProcessDiff(objectHistoryDetails.OldJson.ToString(), objectHistoryDetails.NewJson.ToString());
            }

            var objectHistoryJson = _objectHistoryService.JsonHelper.GetJson(objectHistoryDetails);

            await _objectHistoryService.AzureStorageHelper.UploadJsonFileAsync(objectHistoryContainer, objectHistoryDetails.Folder, "ObjectHistory.json", objectHistoryJson);

            await _objectHistoryService.AzureStorageHelper.AddObjectHistoryEntityRecordAsync(objectHistoryDetails, objectHistoryTable);

            await _objectHistoryService.AzureStorageHelper.AddObjectHistoryGlobalAsync(objectHistoryDetails, globalHistoryTable);

            await blob.DeleteAsync();
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
