using ConsoleApp.ObjectHistoryClient.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Arragro.ObjectHistory.ObjectHistoryClientProvider
{
    public class ObjectHistoryClient
    {
        private readonly string _storageConnectionString;

        private readonly CloudStorageAccount _account;
        private readonly CloudBlobClient _blobClient;
        private readonly CloudQueueClient _queueClient;
        private readonly CloudBlobContainer _objectContainer;
        private readonly CloudQueue _queue;
        private readonly Newtonsoft.Json.JsonSerializerSettings _jsonSettings;

        public ObjectHistoryClient(
            string storageConnectionString,
            string objectContainerName = "trackedobjects",
            string messageQueueName = "objectprocessor")
        {
            _storageConnectionString = storageConnectionString;
            _account = CloudStorageAccount.Parse(_storageConnectionString);
            _blobClient = _account.CreateCloudBlobClient();
            _queueClient = _account.CreateCloudQueueClient();

            _queue = _queueClient.GetQueueReference(messageQueueName);
            _queue.CreateIfNotExistsAsync().Wait();

            _objectContainer = _blobClient.GetContainerReference(objectContainerName);
            _objectContainer.CreateIfNotExistsAsync().Wait();
            _jsonSettings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.All
            };

        }

        public async Task SaveObjectHistoryAsync<T>(Func<string> getKeys, T oldObject, T newObject)
        {
            var fullyQualifiedName = typeof(T).FullName;
            var key = getKeys();
            var partitionKey = $"{fullyQualifiedName}-{key}";
            var objectHistoryJson = String.Empty;

            try
            {
                var oldJson = JsonConvert.SerializeObject(oldObject, Formatting.Indented, _jsonSettings);
                var newJson = JsonConvert.SerializeObject(newObject, Formatting.Indented, _jsonSettings);
                objectHistoryJson = String.Format("{{ \n \"new\":{0},\n \"old\":{1} \n}}", newJson, oldJson);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("There was an issue with serializing the object to json, please review the exception and retry. - {0}", ex.InnerException));
            }

            var trackedObjectMessage = new TrackedObjectMessage(partitionKey, 
                                            string.Format("{0:D19}", DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks), 
                                            DateTime.UtcNow.ToString(),
                                            Guid.NewGuid().ToString());

            var trackedObjectMessageJson = JsonConvert.SerializeObject(trackedObjectMessage, Formatting.Indented);
            
            await UploadJsonFileAsync(trackedObjectMessage.Folder, "objecthistory.json", objectHistoryJson);
            
            await SendQueueMessage(trackedObjectMessageJson);
        }

        private async Task UploadJsonFileAsync(string folder, string fileName, string objectHistoryJson)
        {
            try
            {
                var options = new BlobRequestOptions()
                {
                    ServerTimeout = TimeSpan.FromMinutes(10)
                };

                CloudBlockBlob cloudBlockBlob = _objectContainer.GetBlockBlobReference($"{folder}/{fileName}");

                using (var ms = new MemoryStream())
                {
                    LoadStreamWithJson(ms, objectHistoryJson);
                    await cloudBlockBlob.UploadFromStreamAsync(ms);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void LoadStreamWithJson(Stream ms, string file)
        {
            StreamWriter writer = new StreamWriter(ms);
            writer.Write(file);
            writer.Flush();
            ms.Position = 0;
        }

        private async Task SendQueueMessage(string message)
        {
            try
            {

                var cloudQueueMessage = new CloudQueueMessage(message);
                await _queue.AddMessageAsync(cloudQueueMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void GetObjectHistory()
        {
            throw new NotImplementedException();
        }
    }
}
