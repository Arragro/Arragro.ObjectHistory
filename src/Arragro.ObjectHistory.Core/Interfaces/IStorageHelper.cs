using Arragro.ObjectHistory.Core.Models;
using Azure.Storage.Blobs;
using System;
using System.Threading.Tasks;

namespace Arragro.ObjectHistory.Core.Interfaces
{
    public interface IStorageHelper
    {
        Task AddObjectHistoryEntityRecordAsync(ObjectHistoryDetailBase objectHistoryDetails);
        Task AddObjectHistoryGlobalAsync(ObjectHistoryDetailBase objectHistoryDetails);
        Task<string> DownloadBlob(Guid folder, Guid? subFolder, string filename);
        Task<ObjectHistoryEntity> GetLastObjectHistoryEntity(string partitionKey);
        Task<BlobClient> GetBlobAsync(string blobName);
        Task<Guid?> GetLatestBlobFolderNameByPartitionKeyAsync(string partitionKey);
        Task<ObjectHistoryEntity> GetObjectHistoryRecord(string partitionKey, string rowKey);
        Task<ObjectHistoryQueryResultContainer> GetObjectHistoryRecordsByApplicationNamePartitionKey(string partitionKey, PagingToken pagingToken);
        Task<ObjectHistoryQueryResultContainer> GetObjectHistoryRecordsByObjectNamePartitionKey(string partitionKey, PagingToken pagingToken);
        Task SendQueueMessage(string message);
        Task UploadJsonFileAsync(Guid folder, Guid? subfolder, string fileName, string objectHistoryJson);
    }
}