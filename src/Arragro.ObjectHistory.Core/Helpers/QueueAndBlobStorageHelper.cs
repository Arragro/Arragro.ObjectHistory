using Arragro.ObjectHistory.Core.Extentions;
using Arragro.ObjectHistory.Core.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Arragro.ObjectHistory.Core.Helpers
{
    public class QueueAndBlobStorageHelper
    {
        protected readonly ObjectHistorySettings _objectHistorySettings;

        protected async Task<QueueClient> GetQueueClientAsync()
        {
            var queueClient = new QueueClient(_objectHistorySettings.AzureStorageConnectionString, _objectHistorySettings.ObjectQueueName);
            await queueClient.CreateIfNotExistsAsync();
            return queueClient;
        }

        public QueueAndBlobStorageHelper(ObjectHistorySettings objectHistorySettings)
        {
            _objectHistorySettings = objectHistorySettings;
        }

        protected async Task<BlobContainerClient> GetObjectHistoryOutputContainerAsync()
        {
            var containerClient = new BlobContainerClient(_objectHistorySettings.AzureStorageConnectionString, _objectHistorySettings.ObjectOutputContainerName);
            await containerClient.CreateIfNotExistsAsync();
            return containerClient;
        }

        private void LoadStreamWithJson(Stream ms, string file)
        {
            StreamWriter writer = new StreamWriter(ms);
            writer.Write(file);
            writer.Flush();
            ms.Position = 0;
        }

        public async Task UploadJsonFileAsync(Guid folder, Guid? subfolder, string fileName, string objectHistoryJson)
        {
            try
            {
                var containerClient = await GetObjectHistoryOutputContainerAsync();
                var blobClient = containerClient.GetBlobClient(subfolder.HasValue ? $"{folder}/{subfolder}/{fileName}" : $"{folder}/{fileName}");

                using (var ms = new MemoryStream())
                {
                    LoadStreamWithJson(ms, objectHistoryJson);
                    await blobClient.UploadAsync(ms);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task SendQueueMessage(string message)
        {
            try
            {
                var queueClient = await GetQueueClientAsync();
                await queueClient.SendMessageAsync(JsonConvert.SerializeObject(new ObjectHistoryMessge { Message = message }));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> DownloadBlob(Guid folder, Guid? subFolder, string filename)
        {
            try
            {
                var containerClient = await GetObjectHistoryOutputContainerAsync();
                var blobClient = containerClient.GetBlobClient(subFolder.HasValue ? $"{folder}/{subFolder}/{filename}" : $"{folder}/{filename}");
                return await blobClient.DownloadTextAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<BlobClient> GetBlobAsync(string blobName)
        {
            var containerClient = await GetObjectHistoryOutputContainerAsync();

            var blob = containerClient.GetBlobClient(blobName);

            if (!(await blob.ExistsAsync()))
                throw new Exception($"Blob file {blobName} in queue does not exist in the container.");

            if (!blob.Name.EndsWith(".json"))
                throw new Exception($"Blob file extension for {blobName} is not .json ");

            return blob;
        }
    }
}
