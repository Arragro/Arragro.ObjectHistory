using Arragro.ObjectHistory.Core.Helpers;
using Arragro.ObjectHistory.Core.Models;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading.Tasks;

namespace Arragro.ObjectHistory.Client
{
    public class ObjectHistoryClient
    {
        private readonly ObjectHistorySettings _objectHistorySettings;
        private readonly ObjectHistoryService _objectHistoryService;

        public ObjectHistoryClient(ObjectHistorySettings objectHistorySettings)
        {
            _objectHistorySettings = objectHistorySettings;
            _objectHistoryService = new ObjectHistoryService(objectHistorySettings);
        }

        public async Task SaveNewObjectHistoryAsync<T>(Func<string> getKeys, T newObject, string user)
        {
            var fullyQualifiedName = typeof(T).FullName;

            var key = getKeys();
            var partitionKey = $"{fullyQualifiedName}-{key}";

            var trackedObject = new ObjectHistoryDetailRaw(partitionKey,
                                string.Format("{0:D19}",
                                DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks),
                                _objectHistoryService.ApplicationName,
                                DateTime.UtcNow,
                                user,
                                Guid.NewGuid(),
                                true)
            {
                OldJson = null,
                NewJson = _objectHistoryService.JsonHelper.GetJson(newObject, true)
            };

            var trackedObjectJson = _objectHistoryService.JsonHelper.GetJson(trackedObject);

            await _objectHistoryService.AzureStorageHelper.UploadJsonFileAsync(_objectHistoryService.ObjectContainer, trackedObject.Folder, ObjectHistoryService.ObjectHistoryRequestFileName, trackedObjectJson);

            await _objectHistoryService.AzureStorageHelper.SendQueueMessage(_objectHistoryService.Queue, String.Format("{0}/{1}", trackedObject.Folder, ObjectHistoryService.ObjectHistoryRequestFileName));

        }

        public async Task SaveObjectHistoryAsync<T>(Func<string> getKeys, T oldObject, T newObject, string user)
        {
            var fullyQualifiedName = typeof(T).FullName;

            var key = getKeys();
            var partitionKey = $"{fullyQualifiedName}-{key}";

            var trackedObject = new ObjectHistoryDetailRaw(partitionKey,
                                string.Format("{0:D19}", 
                                DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks),
                                _objectHistoryService.ApplicationName,
                                DateTime.UtcNow, 
                                user, 
                                Guid.NewGuid(),
                                false)
            {
                OldJson = _objectHistoryService.JsonHelper.GetJson(oldObject, true),
                NewJson = _objectHistoryService.JsonHelper.GetJson(newObject, true)
            };

            var trackedObjectJson = _objectHistoryService.JsonHelper.GetJson(trackedObject);
            
            await _objectHistoryService.AzureStorageHelper.UploadJsonFileAsync(_objectHistoryService.ObjectContainer, trackedObject.Folder, ObjectHistoryService.ObjectHistoryRequestFileName, trackedObjectJson);
            
            await _objectHistoryService.AzureStorageHelper.SendQueueMessage(_objectHistoryService.Queue, String.Format("{0}/{1}",trackedObject.Folder, ObjectHistoryService.ObjectHistoryRequestFileName));
        }
        
        public async Task<string> GetObjectHistoryRecordsOfSuppliedTypeAndId<T>(Func<string> getKeys, TableContinuationToken continuationToken = null)
        {
            var fullyQualifiedName = typeof(T).FullName;
            var key = getKeys();
            var partitionKey = $"{fullyQualifiedName}-{key}";

            var entities = _objectHistoryService.JsonHelper.GetJson( await _objectHistoryService.AzureStorageHelper.GetObjectHistoryRecordsByObjectNamePartitionKey(partitionKey, _objectHistoryService.Table, null));

            return entities;
        }

        public async Task<ObjectHistoryQueryResultContainer> GetObjectHistoryRecordsByObjectNamePartitionKey(string partitionKey, TableContinuationToken continuationToken = null)
        {
            return  await _objectHistoryService.AzureStorageHelper.GetObjectHistoryRecordsByObjectNamePartitionKey(partitionKey, _objectHistoryService.Table, continuationToken);
        }

        public async Task<ObjectHistoryQueryResultContainer> GetObjectHistoryRecordsByApplicationNamePartitionKey(TableContinuationToken continuationToken = null)
        {
            return await _objectHistoryService.AzureStorageHelper.GetObjectHistoryRecordsByApplicationNamePartitionKey(_objectHistorySettings.ApplicationName, _objectHistoryService.GlobalTable, continuationToken);
        }

        public async Task<string> GetObjectHistoryFile(string folder)
        {
            return await _objectHistoryService.AzureStorageHelper.DownloadBlob(_objectHistoryService.ObjectContainer, folder, ObjectHistoryService.ObjectHistoryFileName);
        }

        public async Task<ObjectHistoryDetailRaw> GetObjectHistoryDetailRaw(Guid folder)
        {
            var json = await _objectHistoryService.AzureStorageHelper.DownloadBlob(_objectHistoryService.ObjectContainer, folder.ToString(), ObjectHistoryService.ObjectHistoryFileName);
            var read = _objectHistoryService.JsonHelper.GetObjectFromJson<ObjectHistoryDetailRead>(json);

            return read.GetObjectHistoryDetailRaw();
        }
    }
}
