using Arragro.ObjectHistory.Core.Interfaces;
using Arragro.ObjectHistory.Core.Models;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Arragro.ObjectHistory.Core.Helpers
{
    public class AzureStorageHelper : QueueAndBlobStorageHelper, IStorageHelper
    {
        public AzureStorageHelper(ObjectHistorySettings objectHistorySettings) : base(objectHistorySettings)
        {
        }

        private async Task<CloudTable> GetObjectHistoryTableAsync()
        {
            var tableClient = CloudStorageAccount.Parse(_objectHistorySettings.AzureStorageConnectionString).CreateCloudTableClient();
            var table = tableClient.GetTableReference(_objectHistorySettings.ObjectHistoryTable);
            await table.CreateIfNotExistsAsync();
            return table;
        }

        private async Task<CloudTable> GetGlobalHistoryTableAsync()
        {
            var tableClient = CloudStorageAccount.Parse(_objectHistorySettings.AzureStorageConnectionString).CreateCloudTableClient();
            var table = tableClient.GetTableReference(_objectHistorySettings.GlobalHistoryTable);
            await table.CreateIfNotExistsAsync();
            return table;
        }

        public async Task AddObjectHistoryEntityRecordAsync(ObjectHistoryDetailBase objectHistoryDetails)
        {
            try
            {
                var cloudTable = await GetObjectHistoryTableAsync();
                var objectHistoryEntity = new ObjectHistoryTableEntity(objectHistoryDetails.PartitionKey, objectHistoryDetails.RowKey)
                {
                    ApplicationName = objectHistoryDetails.ApplicationName,
                    User = objectHistoryDetails.User,
                    Folder = objectHistoryDetails.Folder
                };

                var insertOperation = TableOperation.Insert(objectHistoryEntity);
                await cloudTable.ExecuteAsync(insertOperation);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Somthing has gone wrong with the adding of the table record. Please review the inner exception. {0}", ex.InnerException));
            }
        }

        public async Task AddObjectHistoryGlobalAsync(ObjectHistoryDetailBase objectHistoryDetails)
        {
            try
            {
                var cloudTable = await GetGlobalHistoryTableAsync();
                var objectHistoryEntity = new ObjectHistoryGlobalTableEntity(objectHistoryDetails.ApplicationName, objectHistoryDetails.RowKey)
                {
                    User = objectHistoryDetails.User,
                    ObjectName = objectHistoryDetails.PartitionKey,
                    Folder = objectHistoryDetails.Folder
                };

                var insertOperation = TableOperation.Insert(objectHistoryEntity);
                await cloudTable.ExecuteAsync(insertOperation);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Somthing has gone wrong with the adding of the table record. Please review the inner exception. {0}", ex.InnerException));
            }
        }

        public async Task<ObjectHistoryEntity> GetLastObjectHistoryEntity(string partitionKey)
        {
            var cloudTable = await GetObjectHistoryTableAsync();
            var query = new TableQuery<ObjectHistoryTableEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));
            query = query.Take(1);
            query.TakeCount = 1;

            var result = await cloudTable.ExecuteQuerySegmentedAsync(query, null);
            var objectHistoryTableEntity = result.FirstOrDefault();
            if (objectHistoryTableEntity == null)
                return null;
            return new ObjectHistoryEntity(objectHistoryTableEntity);
        }

        public async Task<Guid?> GetLatestBlobFolderNameByPartitionKeyAsync(string partitionKey)
        {
            try
            {
                var objectHistoryEntity = await GetLastObjectHistoryEntity(partitionKey);
                if (objectHistoryEntity == null)
                    return null;

                return objectHistoryEntity.Folder;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ObjectHistoryEntity> GetObjectHistoryRecord(string partitionKey, string rowKey)
        {
            try
            {
                var cloudTable = await GetObjectHistoryTableAsync();
                var retrieveOperation = TableOperation.Retrieve<ObjectHistoryTableEntity>(partitionKey, rowKey);

                var retrievedResult = await cloudTable.ExecuteAsync(retrieveOperation);

                return new ObjectHistoryEntity(retrievedResult.Result as ObjectHistoryTableEntity);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ObjectHistoryQueryResultContainer> GetObjectHistoryRecordsByObjectNamePartitionKey(string partitionKey, PagingToken pagingToken)
        {
            try
            {
                var query = new TableQuery<ObjectHistoryTableEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));

                var cloudTable = await GetObjectHistoryTableAsync();
                var queryResult = await cloudTable.ExecuteQuerySegmentedAsync(query.Take(10), pagingToken.TableContinuationToken);
                var tableContinuationToken = queryResult.ContinuationToken;

                var entityResults = new ObjectHistoryQueryResultContainer(queryResult.Results.Select(x => new ObjectHistoryEntity(x)), tableContinuationToken, partitionKey);

                return entityResults;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ObjectHistoryQueryResultContainer> GetObjectHistoryRecordsByApplicationNamePartitionKey(string partitionKey, PagingToken pagingToken)
        {
            try
            {
                var query = new TableQuery<ObjectHistoryGlobalTableEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));

                var cloudTable = await GetGlobalHistoryTableAsync();
                var queryResult = await cloudTable.ExecuteQuerySegmentedAsync(query.Take(10), pagingToken.TableContinuationToken);
                var tableContinuationToken = queryResult.ContinuationToken;

                var entityResults = new ObjectHistoryQueryResultContainer(queryResult.Results.Select(x => new ObjectHistoryGlobalEntity(x)), tableContinuationToken, partitionKey);

                return entityResults;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
