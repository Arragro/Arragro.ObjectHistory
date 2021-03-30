using Arragro.ObjectHistory.Core.Extentions;
using Arragro.ObjectHistory.Core.Interfaces;
using Arragro.ObjectHistory.Core.Models;
using JsonDiffPatchDotNet;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace Arragro.ObjectHistory.Core.Helpers
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

        public async Task ProcessQueueMessageAsync(string blobName)
        {
            if (string.IsNullOrWhiteSpace(blobName))
                throw new ArgumentNullException("blobName");

            var blob = await _storageHelper.GetBlobAsync(blobName);

            var objectHistoryDetailsJson = await blob.DownloadTextAsync();

            var objectHistoryDetails = _jsonHelper.GetObjectFromJson<ObjectHistoryDetailRead>(objectHistoryDetailsJson);

            await ProcessObjectHistoryDetailAsync(objectHistoryDetails);

            await blob.DeleteAsync();
        }

        public async Task ProcessObjectHistoryDetailAsync(ObjectHistoryDetailRead objectHistoryDetails)
        {
            if (!objectHistoryDetails.IsAdd)
            {
                objectHistoryDetails.Diff = ProcessDiff(objectHistoryDetails.OldJson.ToString(), objectHistoryDetails.NewJson.ToString()).ToString();
            }

            var objectHistoryJson = _jsonHelper.GetJson(objectHistoryDetails);

            await _storageHelper.UploadJsonFileAsync(objectHistoryDetails.Folder, objectHistoryDetails.SubFolder, Constants.ObjectHistoryFileName, objectHistoryJson);

            await _storageHelper.AddObjectHistoryEntityRecordAsync(objectHistoryDetails);

            await _storageHelper.AddObjectHistoryGlobalAsync(objectHistoryDetails);
        }

        public async Task ProcessObjectHistoryDeletedDetailAsync(ObjectHistoryDetailRead objectHistoryDetails)
        {
            var objectHistoryJson = _jsonHelper.GetJson(objectHistoryDetails);

            await _storageHelper.UploadJsonFileAsync(objectHistoryDetails.Folder, objectHistoryDetails.SubFolder, Constants.ObjectHistoryDeletedFileName, objectHistoryJson);

            await _storageHelper.AddObjectHistoryDeletedEntityRecordAsync(objectHistoryDetails);
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
