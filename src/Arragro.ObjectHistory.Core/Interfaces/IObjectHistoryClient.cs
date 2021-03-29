using System;
using System.Threading.Tasks;
using Arragro.ObjectHistory.Core.Models;

namespace Arragro.ObjectHistory.Core
{
    public interface IObjectHistoryClient
    {
        Task<ObjectHistoryDetailRaw> GetObjectHistoryDetailRawAsync(string partitionKey);
        Task<ObjectHistoryDetailRaw> GetObjectHistoryDetailRawAsync(string partitionKey, string rowKey);
        Task<ObjectHistoryQueryResultContainer> GetObjectHistoryRecordsByApplicationNamePartitionKeyAsync(PagingToken pagingToken = null);
        Task<ObjectHistoryQueryResultContainer> GetObjectHistoryRecordsByObjectNamePartitionKeyAsync(string partitionKey, PagingToken pagingToken = null);
        Task QueueObjectHistoryAsync<T>(Func<string> getKeys, T newObject, string user, Guid? folder = null);
        Task QueueObjectHistoryAsync<T>(Func<string> getKeys, T oldObject, T newObject, string user, Guid? folder = null);
        Task SaveObjectHistoryAsync<T>(Func<string> getKeys, T newObject, string user, Guid? folder = null);
    }
}