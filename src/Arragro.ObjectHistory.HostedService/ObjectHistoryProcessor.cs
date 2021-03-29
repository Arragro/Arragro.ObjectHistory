using Arragro.ObjectHistory.Client;
using Arragro.ObjectHistory.Core.Extentions;
using Arragro.ObjectHistory.Core.Helpers;
using Arragro.ObjectHistory.Core.Interfaces;
using Arragro.ObjectHistory.Core.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Queues.Models;
using JsonDiffPatchDotNet;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace Arragro.ObjectHistory
{
    public class ObjectHistoryProcessor
    {
        private readonly IStorageHelper _storageHelper;
        private readonly JsonHelper _jsonHelper;

        public ObjectHistoryProcessor(
            IStorageHelper storageHelper)
        {
            _storageHelper = storageHelper;
            _jsonHelper = new JsonHelper();
        }

        public async Task ProcessMessagesAsync(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException("message");

            await ValidateAndProcessQueueMessage(message);
        }

        private async Task ValidateAndProcessQueueMessage(string blobName)
        {
            var blob = await _storageHelper.GetInputBlobAsync(blobName);

            var objectHistoryDetailsJson = await blob.DownloadTextAsync();

            var objectHistoryDetails = _jsonHelper.GetObjectFromJson<ObjectHistoryDetailRead>(objectHistoryDetailsJson);

            if (!objectHistoryDetails.IsAdd)
            {
                objectHistoryDetails.Diff = ProcessDiff(objectHistoryDetails.OldJson.ToString(), objectHistoryDetails.NewJson.ToString());
            }

            var objectHistoryJson = _jsonHelper.GetJson(objectHistoryDetails);

            await _storageHelper.UploadJsonFileAsync(objectHistoryDetails.Folder, objectHistoryDetails.SubFolder, "ObjectHistory.json", objectHistoryJson);

            await _storageHelper.AddObjectHistoryEntityRecordAsync(objectHistoryDetails);

            await _storageHelper.AddObjectHistoryGlobalAsync(objectHistoryDetails);

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
