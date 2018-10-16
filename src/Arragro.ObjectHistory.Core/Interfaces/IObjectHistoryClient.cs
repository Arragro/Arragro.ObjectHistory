using System;
using System.Threading.Tasks;
using Arragro.ObjectHistory.Core.Models;
using Microsoft.WindowsAzure.Storage.Table;

namespace Arragro.ObjectHistory.Core
{
    public interface IObjectHistoryClient
    {
        Task<ObjectHistoryDetailRaw> GetObjectHistoryDetailRawAsync(string partitionKey);
        Task<ObjectHistoryDetailRaw> GetObjectHistoryDetailRawAsync(string partitionKey, string rowKey);
        Task<string> GetObjectHistoryFileAsync(string folder);
        Task<ObjectHistoryQueryResultContainer> GetObjectHistoryRecordsByApplicationNamePartitionKeyAsync(TableContinuationToken continuationToken = null);
        Task<ObjectHistoryQueryResultContainer> GetObjectHistoryRecordsByObjectNamePartitionKeyAsync(string partitionKey, TableContinuationToken continuationToken = null);
        Task<string> GetObjectHistoryRecordsOfSuppliedTypeAndIdAsync<T>(Func<string> getKeys, TableContinuationToken continuationToken = null);
        Task SaveNewObjectHistoryAsync<T>(Func<string> getKeys, T newObject, string user);
        Task SaveObjectHistoryAsync<T>(Func<string> getKeys, T oldObject, T newObject, string user);
    }
}