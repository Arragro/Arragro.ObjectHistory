using Arragro.ObjectHistory.Core;
using Arragro.ObjectHistory.Core.Extentions;
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
        private readonly ObjectHistoryProcessor _objectHistoryProcessor;
        private readonly JsonHelper _jsonHelper;

        public ObjectHistoryClient(
            ObjectHistorySettings objectHistorySettings, 
            IStorageHelper storageHelper,
            ObjectHistoryProcessor objectHistoryProcessor)
        {
            _objectHistorySettings = objectHistorySettings;
            _storageHelper = storageHelper;
            _objectHistoryProcessor = objectHistoryProcessor;
            _jsonHelper = new JsonHelper();
        }

        private ObjectHistoryDetailRaw GetObjectHistoryDetailRaw<T>(Func<string> getKeys, string user, bool isAdd, Guid? folder = null)
        {
            var fullyQualifiedName = typeof(T).FullName;

            var key = getKeys();
            var partitionKey = $"{fullyQualifiedName}-{key}";

            if (folder.HasValue)
                return new ObjectHistoryDetailRaw(
                                    _objectHistorySettings.ToObjectHistorySettingsBase(),
                                    partitionKey,
                                    _objectHistorySettings.ApplicationName,
                                    user,
                                    folder: folder.Value,
                                    isAdd: isAdd);
            else
                return new ObjectHistoryDetailRaw(
                                    _objectHistorySettings.ToObjectHistorySettingsBase(),
                                    partitionKey,
                                    _objectHistorySettings.ApplicationName,
                                    user,
                                    folder: null,
                                    isAdd: isAdd);
        }

        private async Task QueueObjectHistoryAsync(ObjectHistoryDetailRaw objectHistoryDetailRaw)
        {
            var trackedObjectJson = _jsonHelper.GetJson(objectHistoryDetailRaw);

            await _storageHelper.UploadJsonFileAsync(objectHistoryDetailRaw.Folder, objectHistoryDetailRaw.SubFolder, Constants.ObjectHistoryRequestFileName, trackedObjectJson);

            await _storageHelper.SendQueueMessage(objectHistoryDetailRaw.SubFolder.HasValue ? $"{objectHistoryDetailRaw.Folder}/{objectHistoryDetailRaw.SubFolder}/{Constants.ObjectHistoryRequestFileName}" : $"{objectHistoryDetailRaw.Folder}/{Constants.ObjectHistoryRequestFileName}");
        }

        public async Task QueueObjectHistoryAsync<T>(Func<string> getKeys, T newObject, string user, Guid? folder = null)
        {
            var objectHistoryDetailRaw = GetObjectHistoryDetailRaw<T>(getKeys, user, true, folder);
            objectHistoryDetailRaw.OldJson = null;
            objectHistoryDetailRaw.NewJson = _jsonHelper.GetJson(newObject, true);
            await QueueObjectHistoryAsync(objectHistoryDetailRaw);
        }

        public async Task QueueObjectHistoryAsync<T>(Func<string> getKeys, T oldObject, T newObject, string user, Guid? folder = null)
        {
            var objectHistoryDetailRaw = GetObjectHistoryDetailRaw<T>(getKeys, user, false, folder);
            objectHistoryDetailRaw.OldJson = _jsonHelper.GetJson(oldObject, true);
            objectHistoryDetailRaw.NewJson = _jsonHelper.GetJson(newObject, true);
            await QueueObjectHistoryAsync(objectHistoryDetailRaw);
        }

        public async Task SaveObjectHistoryAsync<T>(Func<string> getKeys, T newObject, string user, Guid? folder = null)
        {
            var objectHistoryDetailRaw = GetObjectHistoryDetailRaw<T>(getKeys, user, false, folder);
            var current = await _storageHelper.GetLastObjectHistoryEntity($"{typeof(T).FullName}-{getKeys()}");
            if (current != null)
            {
                var blobClient = await _storageHelper.GetBlobAsync($"{current.GetBlobPath()}/{Constants.ObjectHistoryFileName}");
                if (await blobClient.ExistsAsync())
                {
                    var objectHistoryDetailsJson = await blobClient.DownloadTextAsync();
                    var objectHistoryDetails = _jsonHelper.GetObjectFromJson<ObjectHistoryDetailRead>(objectHistoryDetailsJson);
                    objectHistoryDetailRaw.OldJson = objectHistoryDetails.NewJson.ToString();
                }
                else
                    objectHistoryDetailRaw.OldJson = null;
            }
            else
                objectHistoryDetailRaw.OldJson = null;
            objectHistoryDetailRaw.NewJson = _jsonHelper.GetJson(newObject, true);

            await _objectHistoryProcessor.ProcessObjectHistoryDetailAsync(new ObjectHistoryDetailRead( objectHistoryDetailRaw));
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
