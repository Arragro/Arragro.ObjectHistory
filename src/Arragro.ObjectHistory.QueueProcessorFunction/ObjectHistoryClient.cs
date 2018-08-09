using Arragro.ObjectHistory.Core.Helpers;
using Arragro.ObjectHistory.Core.Models;
using JsonDiffPatchDotNet;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace Arragro.ObjectHistory.QueueProcessorFunction
{
    public class ObjectHistoryClient
    {
        private readonly ObjectHistoryService _objectHistoryService;

        public ObjectHistoryClient(ObjectHistorySettings configurationSettings)
        {
            _objectHistoryService = new ObjectHistoryService(configurationSettings);
        }

        public async Task ProcessMessages()
        {
            var message = GetQueueMessage();

            if (message != null)
            {
                await ValidateAndProcessQueueMessage(message.AsString);
                await _objectHistoryService.Queue.DeleteMessageAsync(message);
            }
        }

        private CloudQueueMessage GetQueueMessage()
        {
            var message = _objectHistoryService.Queue.GetMessageAsync();

            return message.Result;
        }

        private CloudBlockBlob GetObjectHistoryBlob(string blobName)
        {
            try
            {
                var blob = _objectHistoryService.ObjectContainer.GetBlockBlobReference(blobName);

                if (!blob.ExistsAsync().Result)
                    throw new Exception("Blob file {0} in queue does not exist in the container.");

                if (!blob.Name.EndsWith(".json"))
                    throw new Exception("Blob file extension for {0} is not .json ");

                return blob;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task ValidateAndProcessQueueMessage(string blobName)
        {
            var blob = GetObjectHistoryBlob(blobName);

            var objectHistoryDetailsJson = blob.DownloadTextAsync().Result;
            var objectHistoryDetails = _objectHistoryService.JsonHelper.GetObjectFromJson<ObjectHistoryDetailRead>(objectHistoryDetailsJson);

            if (!objectHistoryDetails.IsAdd)
            {
                objectHistoryDetails.Diff = ProcessDiff(objectHistoryDetails.OldJson.ToString(), objectHistoryDetails.NewJson.ToString());
                await CheckAndUpdateHistory(objectHistoryDetails.PartitionKey, _objectHistoryService.Table, objectHistoryDetails);
            }

            var objectHistoryJson = _objectHistoryService.JsonHelper.GetJson(objectHistoryDetails);

            await _objectHistoryService.AzureStorageHelper.UploadJsonFileAsync(_objectHistoryService.ObjectContainer, objectHistoryDetails.Folder, "ObjectHistory.json", objectHistoryJson);

            await _objectHistoryService.AzureStorageHelper.AddObjectHistoryEntityRecord(objectHistoryDetails, _objectHistoryService.Table);

            await _objectHistoryService.AzureStorageHelper.AddObjectHistoryGlobal(objectHistoryDetails, _objectHistoryService.GlobalTable);

            await blob.DeleteAsync();

        }

        private async Task CheckAndUpdateHistory(string partitionKey, CloudTable table, ObjectHistoryDetailRead objectHistoryDetail)
        {
            var lastObjectHistorydetailFolder = await _objectHistoryService.AzureStorageHelper.GetLatestBlobFolderNameByPartitionKey(partitionKey, table);

            if (lastObjectHistorydetailFolder != Guid.Empty)
            {
                var blobName = String.Format("{0}/{1}", lastObjectHistorydetailFolder.ToString(), _objectHistoryService.ObjectHistoryFileName);

                var blob = GetObjectHistoryBlob(blobName);

                var lastObjectHistorydetailJson = blob.DownloadTextAsync().Result;
                var lastObjectHistorydetails = _objectHistoryService.JsonHelper.GetObjectFromJson<ObjectHistoryDetailRead>(lastObjectHistorydetailJson);

                var leapDiff = ProcessDiff(objectHistoryDetail.OldJson.ToString(), lastObjectHistorydetails.NewJson.ToString());

                var isDiff = false;

                if (leapDiff != null)
                    isDiff = leapDiff.HasValues;

                if (isDiff)
                {
                    var timespan = objectHistoryDetail.TimeStamp.Subtract(lastObjectHistorydetails.TimeStamp);
                    var catchupTimestamp = lastObjectHistorydetails.TimeStamp.AddSeconds(timespan.TotalSeconds / 2);

                    var trackedObject = new ObjectHistoryDetailRaw(partitionKey,
                               string.Format("{0:D19}",
                               DateTime.MaxValue.Ticks - catchupTimestamp.Ticks),
                               objectHistoryDetail.ApplicationName,
                               catchupTimestamp,
                               "System Update",
                               Guid.NewGuid())
                    {
                        OldJson = _objectHistoryService.JsonHelper.GetJson(lastObjectHistorydetails.OldJson, true),
                        NewJson = _objectHistoryService.JsonHelper.GetJson(objectHistoryDetail.NewJson, true),
                        Diff = _objectHistoryService.JsonHelper.GetJson(leapDiff)
                    };

                    var catchupTrackedObjectJson = _objectHistoryService.JsonHelper.GetJson(trackedObject);
                    await _objectHistoryService.AzureStorageHelper.UploadJsonFileAsync(_objectHistoryService.ObjectContainer, trackedObject.Folder, _objectHistoryService.ObjectHistoryFileName, catchupTrackedObjectJson);


                    await _objectHistoryService.AzureStorageHelper.AddObjectHistoryEntityRecord(trackedObject, _objectHistoryService.Table);

                    await _objectHistoryService.AzureStorageHelper.AddObjectHistoryGlobal(trackedObject, _objectHistoryService.GlobalTable);
                }
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
