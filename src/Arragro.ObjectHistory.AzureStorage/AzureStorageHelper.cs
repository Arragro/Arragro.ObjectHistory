using Arragro.ObjectHistory.Core.Helpers;
using Arragro.ObjectHistory.Core.Interfaces;
using Arragro.ObjectHistory.Core.Models;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Cosmos.Table.Queryable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Arragro.ObjectHistory.AzureStorage
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

        private async Task<CloudTable> GetObjectHistoryDeletedTableAsync()
        {
            var tableClient = CloudStorageAccount.Parse(_objectHistorySettings.AzureStorageConnectionString).CreateCloudTableClient();
            var table = tableClient.GetTableReference(_objectHistorySettings.ObjectHistoryDeletedTable);
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
                    Folder = objectHistoryDetails.Folder,
                    SubFolder = objectHistoryDetails.SubFolder,
                    IsAdd = objectHistoryDetails.IsAdd,
                    SecurityValidationToken = objectHistoryDetails.SecurityValidationToken
                };

                var insertOperation = TableOperation.Insert(objectHistoryEntity);
                await cloudTable.ExecuteAsync(insertOperation);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Somthing has gone wrong with the adding of the table record. Please review the inner exception. {0}", ex.InnerException));
            }
        }

        public async Task AddObjectHistoryDeletedEntityRecordAsync(ObjectHistoryDetailBase objectHistoryDetails)
        {
            try
            {
                var cloudTable = await GetObjectHistoryDeletedTableAsync();
                var objectHistoryEntity = new ObjectHistoryDeletedTableEntity(objectHistoryDetails.PartitionKey, objectHistoryDetails.RowKey)
                {
                    ApplicationName = objectHistoryDetails.ApplicationName,
                    User = objectHistoryDetails.User,
                    Folder = objectHistoryDetails.Folder,
                    SubFolder = objectHistoryDetails.SubFolder,
                    SecurityValidationToken = objectHistoryDetails.SecurityValidationToken
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
                    Folder = objectHistoryDetails.Folder,
                    SubFolder = objectHistoryDetails.SubFolder,
                    IsAdd = objectHistoryDetails.IsAdd,
                    SecurityValidationToken = objectHistoryDetails.SecurityValidationToken
                };

                var insertOperation = TableOperation.Insert(objectHistoryEntity);
                await cloudTable.ExecuteAsync(insertOperation);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Somthing has gone wrong with the adding of the table record. Please review the inner exception. {0}", ex.InnerException));
            }
        }

        public async Task<ObjectHistoryEntity> GetLastObjectHistoryEntityAsync(string partitionKey)
        {
            var cloudTable = await GetObjectHistoryTableAsync();
            var query = new TableQuery<ObjectHistoryTableEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));
            query = query.Take(1);
            query.TakeCount = 1;

            var result = await cloudTable.ExecuteQuerySegmentedAsync(query, null);
            var objectHistoryTableEntity = result.FirstOrDefault();
            if (objectHistoryTableEntity == null)
                return null;
            return new ObjectHistoryEntity(
                objectHistoryTableEntity.PartitionKey,
                objectHistoryTableEntity.RowKey,
                objectHistoryTableEntity.ApplicationName,
                objectHistoryTableEntity.Folder,
                objectHistoryTableEntity.SubFolder,
                objectHistoryTableEntity.Timestamp,
                objectHistoryTableEntity.User,
                objectHistoryTableEntity.IsAdd,
                objectHistoryTableEntity.SecurityValidationToken);
        }

        public async Task<ObjectHistoryDeletedEntity> GetLastObjectHistoryDeletedEntityAsync(string partitionKey)
        {
            var cloudTable = await GetObjectHistoryDeletedTableAsync();
            var query = new TableQuery<ObjectHistoryDeletedTableEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));
            query = query.Take(1);
            query.TakeCount = 1;

            var result = await cloudTable.ExecuteQuerySegmentedAsync(query, null);
            var objectHistoryTableEntity = result.FirstOrDefault();
            if (objectHistoryTableEntity == null)
                return null;
            return new ObjectHistoryDeletedEntity(
                objectHistoryTableEntity.PartitionKey,
                objectHistoryTableEntity.RowKey,
                objectHistoryTableEntity.ApplicationName,
                objectHistoryTableEntity.Folder,
                objectHistoryTableEntity.SubFolder,
                objectHistoryTableEntity.Timestamp,
                objectHistoryTableEntity.User,
                objectHistoryTableEntity.SecurityValidationToken);
        }

        public async Task<Guid?> GetLatestBlobFolderNameByPartitionKeyAsync(string partitionKey)
        {
            try
            {
                var objectHistoryEntity = await GetLastObjectHistoryEntityAsync(partitionKey);
                if (objectHistoryEntity == null)
                    return null;

                return objectHistoryEntity.Folder;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ObjectHistoryEntity> GetObjectHistoryRecordAsync(string partitionKey, string rowKey)
        {
            try
            {
                var cloudTable = await GetObjectHistoryTableAsync();
                var retrieveOperation = TableOperation.Retrieve<ObjectHistoryTableEntity>(partitionKey, rowKey);

                var retrievedResult = await cloudTable.ExecuteAsync(retrieveOperation);
                var objectHistoryTableEntity = retrievedResult.Result as ObjectHistoryTableEntity;

                return new ObjectHistoryEntity(
                    objectHistoryTableEntity.PartitionKey,
                    objectHistoryTableEntity.RowKey,
                    objectHistoryTableEntity.ApplicationName,
                    objectHistoryTableEntity.Folder,
                    objectHistoryTableEntity.SubFolder,
                    objectHistoryTableEntity.Timestamp,
                    objectHistoryTableEntity.User,
                    objectHistoryTableEntity.IsAdd,
                    objectHistoryTableEntity.SecurityValidationToken);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ObjectHistoryQueryResultContainer> GetObjectHistoryDeletedRecordsAsync(PagingToken pagingToken)
        {
            try
            {
                var query = new TableQuery<ObjectHistoryDeletedTableEntity>();
                var cloudTable = await GetObjectHistoryDeletedTableAsync();
                var queryResult = await cloudTable.ExecuteQuerySegmentedAsync(query.Take(10), pagingToken.TableContinuationToken);
                var tableContinuationToken = queryResult.ContinuationToken;

                var entityResults = new ObjectHistoryQueryResultContainer(queryResult.Results.Select(objectHistoryTableEntity =>
                    new ObjectHistoryDeletedEntity(
                        objectHistoryTableEntity.PartitionKey,
                        objectHistoryTableEntity.RowKey,
                        objectHistoryTableEntity.ApplicationName,
                        objectHistoryTableEntity.Folder,
                        objectHistoryTableEntity.SubFolder,
                        objectHistoryTableEntity.Timestamp,
                        objectHistoryTableEntity.User,
                        objectHistoryTableEntity.SecurityValidationToken)),
                    new PagingToken(tableContinuationToken));

                return entityResults;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ObjectHistoryQueryResultContainer> GetObjectHistoryRecordsByObjectNamePartitionKeyAsync(string partitionKey, PagingToken pagingToken)
        {
            try
            {
                var query = new TableQuery<ObjectHistoryTableEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));

                var cloudTable = await GetObjectHistoryTableAsync();
                var queryResult = await cloudTable.ExecuteQuerySegmentedAsync(query.Take(10), pagingToken.TableContinuationToken);
                var tableContinuationToken = queryResult.ContinuationToken;

                var entityResults = new ObjectHistoryQueryResultContainer(queryResult.Results.Select(objectHistoryTableEntity => 
                    new ObjectHistoryEntity(
                        objectHistoryTableEntity.PartitionKey,
                        objectHistoryTableEntity.RowKey,
                        objectHistoryTableEntity.ApplicationName,
                        objectHistoryTableEntity.Folder,
                        objectHistoryTableEntity.SubFolder,
                        objectHistoryTableEntity.Timestamp,
                        objectHistoryTableEntity.User,
                        objectHistoryTableEntity.IsAdd,
                        objectHistoryTableEntity.SecurityValidationToken)), 
                    new PagingToken(tableContinuationToken), partitionKey);

                return entityResults;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ObjectHistoryQueryResultContainer> GetObjectHistoryRecordsByApplicationNamePartitionKeyAsync(string partitionKey, PagingToken pagingToken)
        {
            try
            {
                var query = new TableQuery<ObjectHistoryGlobalTableEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));

                var cloudTable = await GetGlobalHistoryTableAsync();
                var queryResult = await cloudTable.ExecuteQuerySegmentedAsync(query.Take(10), pagingToken.TableContinuationToken);
                var tableContinuationToken = queryResult.ContinuationToken;

                var entityResults = new ObjectHistoryQueryResultContainer(queryResult.Results.Select(objectHistoryGlobalEntity => 
                    new ObjectHistoryGlobalEntity(
                        objectHistoryGlobalEntity.PartitionKey,
                        objectHistoryGlobalEntity.RowKey,
                        objectHistoryGlobalEntity.User,
                        objectHistoryGlobalEntity.ObjectName,
                        objectHistoryGlobalEntity.Folder,
                        objectHistoryGlobalEntity.SubFolder,
                        objectHistoryGlobalEntity.Timestamp)), 
                    new PagingToken(tableContinuationToken), partitionKey);

                return entityResults;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task ProcessEntitiesAsync(CloudTable table, Func<IEnumerable<DynamicTableEntity>, Task> processor, Expression<Func<DynamicTableEntity, bool>> filters)
        {
            TableQuerySegment<DynamicTableEntity> segment = null;

            while (segment == null || segment.ContinuationToken != null)
            {
                if (filters == null)
                {
                    segment = await table.ExecuteQuerySegmentedAsync(new TableQuery().Take(100), segment == null ? null : segment.ContinuationToken);
                }
                else
                {
                    var query = table.CreateQuery<DynamicTableEntity>().Where(filters).Take(100).AsTableQuery();
                    segment = await query.ExecuteSegmentedAsync(segment == null ? null : segment.ContinuationToken);
                }
                segment.Results.ForEach(x =>
                {
                    if (x.ETag == null) x.ETag = "*";
                });

                await processor(segment.Results);
            }
        }

        private async Task DeleteAllEntitiesInBatchesAsync(
            CloudTable table, Expression<Func<DynamicTableEntity, bool>> filter)
        {
            Func<IEnumerable<DynamicTableEntity>, Task> processor = async (entities) =>
            {
                var batches = new Dictionary<string, TableBatchOperation>();

                foreach (var entity in entities)
                {
                    TableBatchOperation batch = null;

                    if (batches.TryGetValue(entity.PartitionKey, out batch) == false)
                    {
                        batches[entity.PartitionKey] = batch = new TableBatchOperation();
                    }

                    batch.Add(TableOperation.Delete(entity));

                    if (batch.Count == 100)
                    {
                        await table.ExecuteBatchAsync(batch);
                        batches[entity.PartitionKey] = new TableBatchOperation();
                    }
                }

                foreach (var batch in batches.Values)
                {
                    if (batch.Count > 0)
                    {
                        table.ExecuteBatch(batch);
                    }
                }
            };

            await ProcessEntitiesAsync(table, processor, filter);
        }

        public async Task DeleteObjectHistoryDeletedByPartitionKey(string partitionKey)
        {
            var cloudTable = await GetObjectHistoryDeletedTableAsync();
            await DeleteAllEntitiesInBatchesAsync(cloudTable, (table) => (table.PartitionKey == partitionKey));
        }
    }
}
