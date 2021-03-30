using Arragro.ObjectHistory.Core.Models;
using Azure.Storage.Blobs;
using System;
using System.Threading.Tasks;

namespace Arragro.ObjectHistory.Core.Interfaces
{
    public interface IStorageHelper
    {
        Task AddObjectHistoryEntityRecordAsync(ObjectHistoryDetailBase objectHistoryDetails);
        Task AddObjectHistoryDeletedEntityRecordAsync(ObjectHistoryDetailBase objectHistoryDetails);
        Task AddObjectHistoryGlobalAsync(ObjectHistoryDetailBase objectHistoryDetails);
        Task<string> DownloadBlobAsync(Guid folder, Guid? subFolder, string filename);
        Task<ObjectHistoryEntity> GetLastObjectHistoryEntityAsync(string partitionKey);
        Task<ObjectHistoryDeletedEntity> GetLastObjectHistoryDeletedEntityAsync(string partitionKey);
        Task<BlobClient> GetBlobAsync(string blobName);
        Task<Guid?> GetLatestBlobFolderNameByPartitionKeyAsync(string partitionKey);
        Task<ObjectHistoryEntity> GetObjectHistoryRecordAsync(string partitionKey, string rowKey);
        Task<ObjectHistoryQueryResultContainer> GetObjectHistoryDeletedRecordsAsync(PagingToken pagingToken);
        Task<ObjectHistoryQueryResultContainer> GetObjectHistoryRecordsByApplicationNamePartitionKeyAsync(string partitionKey, PagingToken pagingToken);
        Task<ObjectHistoryQueryResultContainer> GetObjectHistoryRecordsByObjectNamePartitionKeyAsync(string partitionKey, PagingToken pagingToken);
        Task SendQueueMessageAsync(string message);
        Task UploadJsonFileAsync(Guid folder, Guid? subfolder, string fileName, string objectHistoryJson);
        Task DeleteObjectHistoryDeletedByPartitionKey(string partitionKey);
    }
}