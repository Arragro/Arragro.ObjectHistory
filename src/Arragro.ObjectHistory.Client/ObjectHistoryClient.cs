using Arragro.ObjectHistory.Core.Helpers;
using Arragro.ObjectHistory.Core.Models;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading.Tasks;

namespace Arragro.ObjectHistory.Client
{
    public class ObjectHistoryClient
    {
        private readonly ObjectHistoryClientSettings _objectHistorySettings;
        private readonly ObjectHistoryService _objectHistoryService;

        private readonly CloudTable _objectHistoryTable;
        private readonly CloudTable _globalHistoryTable;
        private readonly CloudBlobContainer _cloudBlobContainer;

        public ObjectHistoryClient(
            ObjectHistoryClientSettings objectHistorySettings, 
            ObjectHistoryService objectHistoryService)
        {
            _objectHistorySettings = objectHistorySettings;
            _objectHistoryService = objectHistoryService;

            _objectHistoryTable = _objectHistoryService.GetObjectHistoryTableAsync(_objectHistorySettings.ObjectHistoryTable).Result;
            _globalHistoryTable = _objectHistoryService.GetGlobalHistoryTableAsync(_objectHistorySettings.GlobalHistoryTable).Result;
            _cloudBlobContainer = _objectHistoryService.GetObjectHistoryContainerAsync(_objectHistorySettings.ObjectContainerName).Result;
        }

        private ObjectHistoryDetailRaw GetObjectHistoryDetailRaw<T>(Func<string> getKeys, string user, bool isAdd)
        {
            var fullyQualifiedName = typeof(T).FullName;

            var key = getKeys();
            var partitionKey = $"{fullyQualifiedName}-{key}";

            return new ObjectHistoryDetailRaw(
                                _objectHistorySettings,
                                partitionKey,
                                _objectHistorySettings.ApplicationName,
                                user,
                                isAdd);
        }

        public async Task SaveNewObjectHistoryAsync<T>(Func<string> getKeys, T newObject, string user)
        {
            var trackedObject = GetObjectHistoryDetailRaw<T>(getKeys, user, true);
            trackedObject.OldJson = null;
            trackedObject.NewJson = _objectHistoryService.JsonHelper.GetJson(newObject, true);

            var trackedObjectJson = _objectHistoryService.JsonHelper.GetJson(trackedObject);

            await _objectHistoryService.AzureStorageHelper.UploadJsonFileAsync(_objectHistoryService.ObjectContainer, trackedObject.Folder, ObjectHistoryService.ObjectHistoryRequestFileName, trackedObjectJson);

            await _objectHistoryService.AzureStorageHelper.SendQueueMessage(_objectHistoryService.Queue, String.Format("{0}/{1}", trackedObject.Folder, ObjectHistoryService.ObjectHistoryRequestFileName));
        }

        public async Task SaveObjectHistoryAsync<T>(Func<string> getKeys, T oldObject, T newObject, string user)
        {
            var trackedObject = GetObjectHistoryDetailRaw<T>(getKeys, user, false);
            trackedObject.OldJson = _objectHistoryService.JsonHelper.GetJson(oldObject, true);
            trackedObject.NewJson = _objectHistoryService.JsonHelper.GetJson(newObject, true);

            var trackedObjectJson = _objectHistoryService.JsonHelper.GetJson(trackedObject);
            
            await _objectHistoryService.AzureStorageHelper.UploadJsonFileAsync(_objectHistoryService.ObjectContainer, trackedObject.Folder, ObjectHistoryService.ObjectHistoryRequestFileName, trackedObjectJson);
            
            await _objectHistoryService.AzureStorageHelper.SendQueueMessage(_objectHistoryService.Queue, String.Format("{0}/{1}",trackedObject.Folder, ObjectHistoryService.ObjectHistoryRequestFileName));
        }
        
        public async Task<string> GetObjectHistoryRecordsOfSuppliedTypeAndIdAsync<T>(Func<string> getKeys, TableContinuationToken continuationToken = null)
        {
            var fullyQualifiedName = typeof(T).FullName;
            var key = getKeys();
            var partitionKey = $"{fullyQualifiedName}-{key}";

            var entities = _objectHistoryService.JsonHelper.GetJson( await _objectHistoryService.AzureStorageHelper.GetObjectHistoryRecordsByObjectNamePartitionKey(partitionKey, _objectHistoryTable, null));

            return entities;
        }

        public async Task<ObjectHistoryQueryResultContainer> GetObjectHistoryRecordsByObjectNamePartitionKeyAsync(string partitionKey, TableContinuationToken continuationToken = null)
        {
            return  await _objectHistoryService.AzureStorageHelper.GetObjectHistoryRecordsByObjectNamePartitionKey(partitionKey, _objectHistoryTable, continuationToken);
        }

        public async Task<ObjectHistoryQueryResultContainer> GetObjectHistoryRecordsByApplicationNamePartitionKeyAsync(TableContinuationToken continuationToken = null)
        {
            return await _objectHistoryService.AzureStorageHelper.GetObjectHistoryRecordsByApplicationNamePartitionKey(_objectHistorySettings.ApplicationName, _globalHistoryTable, continuationToken);
        }

        public async Task<string> GetObjectHistoryFileAsync(string folder)
        {
            return await _objectHistoryService.AzureStorageHelper.DownloadBlob(_objectHistoryService.ObjectContainer, folder, ObjectHistoryService.ObjectHistoryFileName);
        }

        public async Task<ObjectHistoryDetailRaw> GetObjectHistoryDetailRawAsync(string partitionKey, string rowKey)
        {
            var objectHistoryEntity = await _objectHistoryService.AzureStorageHelper.GetObjectHistoryRecord(partitionKey, rowKey, _objectHistoryTable);
            var json = await _objectHistoryService.AzureStorageHelper.DownloadBlob(_cloudBlobContainer, objectHistoryEntity.Folder.ToString(), ObjectHistoryService.ObjectHistoryFileName);
            var read = _objectHistoryService.JsonHelper.GetObjectFromJson<ObjectHistoryDetailRead>(json);

            return read.GetObjectHistoryDetailRaw();
        }

        public async Task<ObjectHistoryDetailRaw> GetObjectHistoryDetailRawAsync(string partitionKey)
        {
            var objectHistoryEntity = await _objectHistoryService.AzureStorageHelper.GetLastObjectHistoryEntity(partitionKey, _objectHistoryTable);
            var json = await _objectHistoryService.AzureStorageHelper.DownloadBlob(_cloudBlobContainer, objectHistoryEntity.Folder.ToString(), ObjectHistoryService.ObjectHistoryFileName);
            var read = _objectHistoryService.JsonHelper.GetObjectFromJson<ObjectHistoryDetailRead>(json);

            return read.GetObjectHistoryDetailRaw();
        }
    }
}
