using Arragro.ObjectHistory.Core;
using Arragro.ObjectHistory.Core.Helpers;
using Arragro.ObjectHistory.Core.Interfaces;
using Arragro.ObjectHistory.Core.Models;
using System;
using System.Threading.Tasks;

namespace Arragro.ObjectHistory.Client
{
    public class ObjectHistoryClient : IObjectHistoryClient
    {
        private readonly ObjectHistorySettings _objectHistorySettings;
        private readonly IStorageHelper _storageHelper;
        private readonly JsonHelper _jsonHelper;

        public ObjectHistoryClient(
            ObjectHistorySettings objectHistorySettings, 
            IStorageHelper storageHelper)
        {
            _objectHistorySettings = objectHistorySettings;
            _storageHelper = storageHelper;
            _jsonHelper = new JsonHelper();
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
            trackedObject.NewJson = _jsonHelper.GetJson(newObject, true);

            var trackedObjectJson = _jsonHelper.GetJson(trackedObject);

            await _storageHelper.UploadJsonFileAsync(trackedObject.Folder, trackedObject.SubFolder,  Constants.ObjectHistoryRequestFileName, trackedObjectJson);

            await _storageHelper.SendQueueMessage(trackedObject.SubFolder.HasValue ? $"{trackedObject.Folder}/{trackedObject.SubFolder}/{Constants.ObjectHistoryRequestFileName}" : $"{trackedObject.Folder}/{Constants.ObjectHistoryRequestFileName}");

        }

        public async Task SaveObjectHistoryAsync<T>(Func<string> getKeys, T oldObject, T newObject, string user)
        {
            var trackedObject = GetObjectHistoryDetailRaw<T>(getKeys, user, false);
            trackedObject.OldJson = _jsonHelper.GetJson(oldObject, true);
            trackedObject.NewJson = _jsonHelper.GetJson(newObject, true);

            var trackedObjectJson = _jsonHelper.GetJson(trackedObject);
            
            await _storageHelper.UploadJsonFileAsync(trackedObject.Folder, trackedObject.SubFolder, Constants.ObjectHistoryRequestFileName, trackedObjectJson);
            
            await _storageHelper.SendQueueMessage(trackedObject.SubFolder.HasValue ? $"{trackedObject.Folder}/{trackedObject.SubFolder}/{Constants.ObjectHistoryRequestFileName}" : $"{trackedObject.Folder}/{Constants.ObjectHistoryRequestFileName}");
        }
        
        public async Task<ObjectHistoryQueryResultContainer> GetObjectHistoryRecordsByObjectNamePartitionKeyAsync(string partitionKey, PagingToken pagingToken = null)
        {
            return  await _storageHelper.GetObjectHistoryRecordsByObjectNamePartitionKey(partitionKey, pagingToken == null ? new PagingToken(null) : pagingToken);
        }

        public async Task<ObjectHistoryQueryResultContainer> GetObjectHistoryRecordsByApplicationNamePartitionKeyAsync(PagingToken pagingToken = null)
        {
            return await _storageHelper.GetObjectHistoryRecordsByApplicationNamePartitionKey(_objectHistorySettings.ApplicationName, pagingToken == null ? new PagingToken(null) : pagingToken);
        }

        public async Task<ObjectHistoryDetailRaw> GetObjectHistoryDetailRawAsync(string partitionKey, string rowKey)
        {
            var objectHistoryEntity = await _storageHelper.GetObjectHistoryRecord(partitionKey, rowKey);
            var json = await _storageHelper.DownloadBlob(objectHistoryEntity.Folder, objectHistoryEntity.SubFolder, Constants.ObjectHistoryFileName);
            var read = _jsonHelper.GetObjectFromJson<ObjectHistoryDetailRead>(json);

            return read.GetObjectHistoryDetailRaw();
        }

        public async Task<ObjectHistoryDetailRaw> GetObjectHistoryDetailRawAsync(string partitionKey)
        {
            var objectHistoryEntity = await _storageHelper.GetLastObjectHistoryEntity(partitionKey);
            if (objectHistoryEntity == null)
                return null;

            var json = await _storageHelper.DownloadBlob(objectHistoryEntity.Folder, objectHistoryEntity.SubFolder, Constants.ObjectHistoryFileName);
            var read = _jsonHelper.GetObjectFromJson<ObjectHistoryDetailRead>(json);

            return read.GetObjectHistoryDetailRaw();
        }
    }
}
