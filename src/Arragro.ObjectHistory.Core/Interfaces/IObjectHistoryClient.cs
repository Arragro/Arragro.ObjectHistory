using System;
using System.Threading.Tasks;
using Arragro.ObjectHistory.Core.Models;

namespace Arragro.ObjectHistory.Core
{
    public interface IObjectHistoryClient
    {
        Task<bool> HasObjectHistoryEntityAsync(string partitionKey);
        Task<ObjectHistoryDetailRaw> GetLatestObjectHistoryDetailRawAsync(string partitionKey);
        Task<ObjectHistoryDetailRaw> GetLatestObjectHistoryDeletedDetailRawAsync(string partitionKey);
        Task<ObjectHistoryDetailRaw> GetObjectHistoryDetailRawAsync(string partitionKey, string rowKey);
        Task<ObjectHistoryQueryResultContainer> GetObjectHistoryRecordsByApplicationNamePartitionKeyAsync(PagingToken pagingToken = null);
        Task<ObjectHistoryQueryResultContainer> GetObjectHistoryRecordsByObjectNamePartitionKeyAsync(string partitionKey, PagingToken pagingToken = null);
        Task QueueObjectHistoryAsync<T>(Func<string> getKeys, T newObject, string user, Guid? folder = null, string metadata= null);
        Task SaveObjectHistoryAsync<T>(Func<string> getKeys, T newObject, string user, Guid? folder = null, string metadata = null);
        Task SaveObjectHistoryDeletedAsync<T>(Func<string> getKeys, T newObject, string user, Guid? folder = null, string metadata = null);
        Task DeletedObjectHistoryDeletedByPartitionKey(string partitionKey);
    }
}