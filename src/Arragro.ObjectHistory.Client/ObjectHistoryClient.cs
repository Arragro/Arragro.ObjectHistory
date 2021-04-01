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

            return new ObjectHistoryDetailRaw(
                                _objectHistorySettings.ToObjectHistorySettingsBase(),
                                partitionKey,
                                _objectHistorySettings.ApplicationName,
                                user,
                                folder: folder,
                                isAdd: isAdd);
        }

        private async Task QueueObjectHistoryAsync(ObjectHistoryDetailRaw objectHistoryDetailRaw)
        {
            var trackedObjectJson = _jsonHelper.GetJson(objectHistoryDetailRaw);

            await _storageHelper.UploadJsonFileAsync(objectHistoryDetailRaw.Folder, objectHistoryDetailRaw.SubFolder, Constants.ObjectHistoryRequestFileName, trackedObjectJson);

            await _storageHelper.SendQueueMessageAsync(objectHistoryDetailRaw.SubFolder.HasValue ? $"{objectHistoryDetailRaw.Folder}/{objectHistoryDetailRaw.SubFolder}/{Constants.ObjectHistoryRequestFileName}" : $"{objectHistoryDetailRaw.Folder}/{Constants.ObjectHistoryRequestFileName}");
        }

        private PagingToken BuildPagingToken(PagingToken pagingToken)
        {
            if (_objectHistorySettings.StorageType == StorageType.AzureStorage)
                return pagingToken == null ? new PagingToken(null) : pagingToken;
            return pagingToken == null ? new PagingToken(1, 10) : pagingToken;
        }

        private async Task<ObjectHistoryDetailRaw> BuildObjectHistoryDataRawAsync<T>(Func<string> getKeys, T newObject, string user, Guid? folder)
        {
            var current = await _storageHelper.GetLatestObjectHistoryEntityAsync($"{typeof(T).FullName}-{getKeys()}");
            var objectHistoryDetailRaw = GetObjectHistoryDetailRaw<T>(getKeys, user, current == null, folder);
            if (current != null)
            {
                var blobClient = await _storageHelper.GetBlobAsync($"{current.GetBlobPath()}/{Constants.ObjectHistoryFileName}");
                if (await blobClient.ExistsAsync())
                {
                    var objectHistoryDetailsJson = await blobClient.DownloadTextAsync();
                    var objectHistoryDetails = _jsonHelper.GetObjectFromJson<ObjectHistoryDetailRead>(objectHistoryDetailsJson);
                    objectHistoryDetailRaw.OldJson = objectHistoryDetails.NewJson.ToString();
                    objectHistoryDetailRaw.Version = current.Version + 1;
                }
                else
                {
                    objectHistoryDetailRaw.IsAdd = true;
                    objectHistoryDetailRaw.OldJson = null;
                    objectHistoryDetailRaw.Version = 1;
                }
            }
            else
            {
                objectHistoryDetailRaw.IsAdd = true;
                objectHistoryDetailRaw.OldJson = null;
                objectHistoryDetailRaw.Version = 1;
            }
            objectHistoryDetailRaw.NewJson = _jsonHelper.GetJson(newObject, true);

            return objectHistoryDetailRaw;
        }

        public async Task QueueObjectHistoryAsync<T>(Func<string> getKeys, T newObject, string user, Guid? folder = null)
        {
            var objectHistoryDetailRaw = await BuildObjectHistoryDataRawAsync(getKeys, newObject, user, folder);
            await QueueObjectHistoryAsync(objectHistoryDetailRaw);
        }

        public async Task SaveObjectHistoryAsync<T>(Func<string> getKeys, T newObject, string user, Guid? folder = null)
        {
            var objectHistoryDetailRaw = await BuildObjectHistoryDataRawAsync(getKeys, newObject, user, folder);
            await _objectHistoryProcessor.ProcessObjectHistoryDetailAsync(new ObjectHistoryDetailRead( objectHistoryDetailRaw));
        }

        public async Task SaveObjectHistoryDeletedAsync<T>(Func<string> getKeys, T newObject, string user, Guid? folder = null)
        {
            var objectHistoryDetailRaw = GetObjectHistoryDetailRaw<T>(getKeys, user, false, folder);
            objectHistoryDetailRaw.NewJson = _jsonHelper.GetJson(newObject, true);

            await _objectHistoryProcessor.ProcessObjectHistoryDeletedDetailAsync(new ObjectHistoryDetailRead(objectHistoryDetailRaw));
        }

        public async Task<ObjectHistoryQueryResultContainer> GetObjectHistoryDeletedRecordsKeyAsync(PagingToken pagingToken = null)
        {
            return await _storageHelper.GetObjectHistoryDeletedRecordsAsync(BuildPagingToken(pagingToken));
        }

        public async Task<ObjectHistoryQueryResultContainer> GetObjectHistoryRecordsByObjectNamePartitionKeyAsync(string partitionKey, PagingToken pagingToken = null)
        {
            return  await _storageHelper.GetObjectHistoryRecordsByObjectNamePartitionKeyAsync(partitionKey, BuildPagingToken(pagingToken));
        }

        public async Task<ObjectHistoryQueryResultContainer> GetObjectHistoryRecordsByApplicationNamePartitionKeyAsync(PagingToken pagingToken = null)
        {
            return await _storageHelper.GetObjectHistoryRecordsByApplicationNamePartitionKeyAsync(_objectHistorySettings.ApplicationName, BuildPagingToken(pagingToken));
        }

        public async Task<ObjectHistoryDetailRaw> GetObjectHistoryDetailRawAsync(string partitionKey, string rowKey)
        {
            var objectHistoryEntity = await _storageHelper.GetObjectHistoryRecordAsync(partitionKey, rowKey);
            var json = await _storageHelper.DownloadBlobAsync(objectHistoryEntity.Folder, objectHistoryEntity.SubFolder, Constants.ObjectHistoryFileName);
            var read = _jsonHelper.GetObjectFromJson<ObjectHistoryDetailRead>(json);

            return read.GetObjectHistoryDetailRaw();
        }

        public async Task<ObjectHistoryDetailRaw> GetLatestObjectHistoryDetailRawAsync(string partitionKey)
        {
            var objectHistoryEntity = await _storageHelper.GetLatestObjectHistoryEntityAsync(partitionKey);
            if (objectHistoryEntity == null)
                return null;

            var json = await _storageHelper.DownloadBlobAsync(objectHistoryEntity.Folder, objectHistoryEntity.SubFolder, Constants.ObjectHistoryFileName);
            var read = _jsonHelper.GetObjectFromJson<ObjectHistoryDetailRead>(json);

            return read.GetObjectHistoryDetailRaw();
        }

        public async Task<ObjectHistoryDetailRaw> GetLatestObjectHistoryDeletedDetailRawAsync(string partitionKey)
        {
            var objectHistoryEntity = await _storageHelper.GetLatestObjectHistoryDeletedEntityAsync(partitionKey);
            if (objectHistoryEntity == null)
                return null;

            var json = await _storageHelper.DownloadBlobAsync(objectHistoryEntity.Folder, objectHistoryEntity.SubFolder, Constants.ObjectHistoryDeletedFileName);
            var read = _jsonHelper.GetObjectFromJson<ObjectHistoryDetailRead>(json);

            return read.GetObjectHistoryDetailRaw();
        }

        public async Task DeletedObjectHistoryDeletedByPartitionKey(string partitionKey)
        {
            await _storageHelper.DeleteObjectHistoryDeletedByPartitionKey(partitionKey);
        }
    }
}
