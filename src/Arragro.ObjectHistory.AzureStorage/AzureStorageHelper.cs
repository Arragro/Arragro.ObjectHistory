using Arragro.ObjectHistory.Core.Helpers;
using Arragro.ObjectHistory.Core.Interfaces;
using Arragro.ObjectHistory.Core.Models;
using Azure;
using Azure.Data.Tables;
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

        public static void EnsureTables(ObjectHistorySettings objectHistorySettings)
        {
            var table = new TableClient(objectHistorySettings.AzureStorageConnectionString, objectHistorySettings.ObjectHistoryTable);
            table.CreateIfNotExists();

            table = new TableClient(objectHistorySettings.AzureStorageConnectionString, objectHistorySettings.ObjectHistoryDeletedTable);
            table.CreateIfNotExists();

            table = new TableClient(objectHistorySettings.AzureStorageConnectionString, objectHistorySettings.GlobalHistoryTable);
            table.CreateIfNotExists();
        }

        private TableClient GetObjectHistoryTable()
        {
            return new TableClient(_objectHistorySettings.AzureStorageConnectionString, _objectHistorySettings.ObjectHistoryTable);
        }

        private TableClient GetObjectHistoryDeletedTable()
        {
            return new TableClient(_objectHistorySettings.AzureStorageConnectionString, _objectHistorySettings.ObjectHistoryDeletedTable);
        }

        private TableClient GetGlobalHistoryTable()
        {
            return new TableClient(_objectHistorySettings.AzureStorageConnectionString, _objectHistorySettings.GlobalHistoryTable);
        }

        public async Task AddObjectHistoryEntityRecordAsync(ObjectHistoryDetailBase objectHistoryDetails)
        {
            try
            {
                var cloudTable = GetObjectHistoryTable();
                var objectHistoryEntity = new ObjectHistoryTableEntity
                {
                    PartitionKey = objectHistoryDetails.PartitionKey,
                    RowKey = objectHistoryDetails.RowKey,
                    Verion = objectHistoryDetails.Version,
                    ApplicationName = objectHistoryDetails.ApplicationName,
                    User = objectHistoryDetails.User,
                    Folder = objectHistoryDetails.Folder,
                    SubFolder = objectHistoryDetails.SubFolder,
                    IsAdd = objectHistoryDetails.IsAdd,
                    SecurityValidationToken = objectHistoryDetails.SecurityValidationToken,
                    Metadata = objectHistoryDetails.Metadata
                };

                await cloudTable.UpsertEntityAsync(objectHistoryEntity, TableUpdateMode.Replace);
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
                var cloudTable = GetObjectHistoryDeletedTable();
                var objectHistoryEntity = new ObjectHistoryDeletedTableEntity()
                {
                    PartitionKey = objectHistoryDetails.PartitionKey,
                    RowKey = objectHistoryDetails.RowKey,
                    Verion = objectHistoryDetails.Version,
                    ApplicationName = objectHistoryDetails.ApplicationName,
                    User = objectHistoryDetails.User,
                    Folder = objectHistoryDetails.Folder,
                    SubFolder = objectHistoryDetails.SubFolder,
                    SecurityValidationToken = objectHistoryDetails.SecurityValidationToken,
                    Metadata = objectHistoryDetails.Metadata
                };

                await cloudTable.UpsertEntityAsync(objectHistoryEntity, TableUpdateMode.Replace);
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
                var cloudTable = GetGlobalHistoryTable();
                var objectHistoryEntity = new ObjectHistoryGlobalTableEntity
                {
                    PartitionKey = objectHistoryDetails.ApplicationName,
                    RowKey = objectHistoryDetails.RowKey,
                    Verion = objectHistoryDetails.Version,
                    User = objectHistoryDetails.User,
                    ObjectName = objectHistoryDetails.PartitionKey,
                    Folder = objectHistoryDetails.Folder,
                    SubFolder = objectHistoryDetails.SubFolder,
                    IsAdd = objectHistoryDetails.IsAdd,
                    SecurityValidationToken = objectHistoryDetails.SecurityValidationToken,
                    Metadata = objectHistoryDetails.Metadata
                };

                await cloudTable.UpsertEntityAsync(objectHistoryEntity, TableUpdateMode.Replace);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Somthing has gone wrong with the adding of the table record. Please review the inner exception. {0}", ex.InnerException));
            }
        }

        public async Task<ObjectHistoryEntity> GetLatestObjectHistoryEntityAsync(string partitionKey)
        {
            var cloudTable = GetObjectHistoryTable();
            var queryResultsFilter = cloudTable.QueryAsync<ObjectHistoryTableEntity>(filter: TableClient.CreateQueryFilter($"PartitionKey eq {partitionKey}"), maxPerPage: 1);
            ObjectHistoryTableEntity objectHistoryTableEntity = null;
            await foreach (ObjectHistoryTableEntity qEntity in queryResultsFilter)
            {
                objectHistoryTableEntity = qEntity;
                break;
            }
            if (objectHistoryTableEntity == null)
                return null;
            return new ObjectHistoryEntity(
                objectHistoryTableEntity.PartitionKey,
                objectHistoryTableEntity.RowKey,
                objectHistoryTableEntity.Verion,
                objectHistoryTableEntity.ApplicationName,
                objectHistoryTableEntity.Folder,
                objectHistoryTableEntity.SubFolder,
                objectHistoryTableEntity.Timestamp,
                objectHistoryTableEntity.User,
                objectHistoryTableEntity.IsAdd,
                objectHistoryTableEntity.Metadata,
                objectHistoryTableEntity.SecurityValidationToken);
        }

        public async Task<ObjectHistoryDeletedEntity> GetLatestObjectHistoryDeletedEntityAsync(string partitionKey)
        {
            var cloudTable = GetObjectHistoryTable();
            var queryResultsFilter = cloudTable.QueryAsync<ObjectHistoryDeletedTableEntity>(filter: TableClient.CreateQueryFilter($"PartitionKey eq {partitionKey}"), maxPerPage: 1);
            ObjectHistoryDeletedTableEntity objectHistoryTableEntity = null;
            await foreach (ObjectHistoryDeletedTableEntity qEntity in queryResultsFilter)
            {
                objectHistoryTableEntity = qEntity;
                break;
            }

            return new ObjectHistoryDeletedEntity(
                objectHistoryTableEntity.PartitionKey,
                objectHistoryTableEntity.RowKey,
                objectHistoryTableEntity.Verion,
                objectHistoryTableEntity.ApplicationName,
                objectHistoryTableEntity.Folder,
                objectHistoryTableEntity.SubFolder,
                objectHistoryTableEntity.Timestamp,
                objectHistoryTableEntity.User,
                objectHistoryTableEntity.Metadata,
                objectHistoryTableEntity.SecurityValidationToken);
        }

        public async Task<Guid?> GetLatestBlobFolderNameByPartitionKeyAsync(string partitionKey)
        {
            try
            {
                var objectHistoryEntity = await GetLatestObjectHistoryEntityAsync(partitionKey);
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
                var cloudTable = GetObjectHistoryTable();
                var retrievedResult = await cloudTable.GetEntityAsync<ObjectHistoryTableEntity>(partitionKey, rowKey);
                var objectHistoryTableEntity = retrievedResult.Value;

                return new ObjectHistoryEntity(
                    objectHistoryTableEntity.PartitionKey,
                    objectHistoryTableEntity.RowKey,
                    objectHistoryTableEntity.Verion,
                    objectHistoryTableEntity.ApplicationName,
                    objectHistoryTableEntity.Folder,
                    objectHistoryTableEntity.SubFolder,
                    objectHistoryTableEntity.Timestamp,
                    objectHistoryTableEntity.User,
                    objectHistoryTableEntity.IsAdd,
                    objectHistoryTableEntity.Metadata,
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
                var cloudTable = GetObjectHistoryDeletedTable();
                var pageable = cloudTable.QueryAsync<ObjectHistoryDeletedTableEntity>(maxPerPage: pagingToken.PageSize);

                var queryResult = new List<ObjectHistoryDeletedTableEntity>();
                string continuationToken = null;
                await foreach (Page<ObjectHistoryDeletedTableEntity> page in pageable.AsPages(continuationToken: pagingToken.TableContinuationToken))
                {
                    queryResult.AddRange(page.Values);
                    continuationToken= page.ContinuationToken;
                    break;
                }

                var entityResults = new ObjectHistoryQueryResultContainer(queryResult.Select(objectHistoryTableEntity =>
                    new ObjectHistoryDeletedEntity(
                        objectHistoryTableEntity.PartitionKey,
                        objectHistoryTableEntity.RowKey,
                        objectHistoryTableEntity.Verion,
                        objectHistoryTableEntity.ApplicationName,
                        objectHistoryTableEntity.Folder,
                        objectHistoryTableEntity.SubFolder,
                        objectHistoryTableEntity.Timestamp,
                        objectHistoryTableEntity.User,
                        objectHistoryTableEntity.Metadata,
                        objectHistoryTableEntity.SecurityValidationToken)),
                    new PagingToken(continuationToken));

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
                var cloudTable = GetObjectHistoryTable();
                var pageable = cloudTable.QueryAsync<ObjectHistoryTableEntity>(filter: $"PartitionKey eq '{partitionKey}'", maxPerPage: pagingToken.PageSize);

                var queryResult = new List<ObjectHistoryTableEntity>();
                string continuationToken = null;
                await foreach (Page<ObjectHistoryTableEntity> page in pageable.AsPages(continuationToken: pagingToken.TableContinuationToken))
                {
                    queryResult.AddRange(page.Values);
                    continuationToken = page.ContinuationToken;
                    break;
                }

                var entityResults = new ObjectHistoryQueryResultContainer(queryResult.Select(objectHistoryTableEntity => 
                    new ObjectHistoryEntity(
                        objectHistoryTableEntity.PartitionKey,
                        objectHistoryTableEntity.RowKey,
                        objectHistoryTableEntity.Verion,
                        objectHistoryTableEntity.ApplicationName,
                        objectHistoryTableEntity.Folder,
                        objectHistoryTableEntity.SubFolder,
                        objectHistoryTableEntity.Timestamp,
                        objectHistoryTableEntity.User,
                        objectHistoryTableEntity.IsAdd,
                        objectHistoryTableEntity.Metadata,
                        objectHistoryTableEntity.SecurityValidationToken)), 
                    pagingToken, partitionKey);

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
                var cloudTable = GetGlobalHistoryTable();
                var pageable = cloudTable.QueryAsync<ObjectHistoryGlobalTableEntity>(filter: $"PartitionKey eq '{partitionKey}'", maxPerPage: pagingToken.PageSize);

                var queryResult = new List<ObjectHistoryGlobalTableEntity>();
                string continuationToken = null;
                await foreach (Page<ObjectHistoryGlobalTableEntity> page in pageable.AsPages(continuationToken: pagingToken.TableContinuationToken))
                {
                    queryResult.AddRange(page.Values);
                    continuationToken = page.ContinuationToken;
                    break;
                }

                var entityResults = new ObjectHistoryQueryResultContainer(queryResult.Select(objectHistoryGlobalEntity => 
                    new ObjectHistoryGlobalEntity(
                        objectHistoryGlobalEntity.PartitionKey,
                        objectHistoryGlobalEntity.RowKey,
                        objectHistoryGlobalEntity.Verion,
                        objectHistoryGlobalEntity.User,
                        objectHistoryGlobalEntity.ObjectName,
                        objectHistoryGlobalEntity.Folder,
                        objectHistoryGlobalEntity.SubFolder,
                        objectHistoryGlobalEntity.Timestamp,
                        objectHistoryGlobalEntity.Metadata)),
                    pagingToken, partitionKey);

                return entityResults;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task ProcessEntitiesAsync(TableClient table, Func<IEnumerable<ITableEntity>, Task> processor, Expression<Func<ITableEntity, bool>> filters)
        {
            var pageable = table.QueryAsync<ObjectHistoryGlobalTableEntity>(maxPerPage: 100);

            var queryResult = new List<ObjectHistoryGlobalTableEntity>();
            string continuationToken = null;
            do
            {
                await foreach (Page<ObjectHistoryGlobalTableEntity> page in pageable.AsPages(continuationToken: continuationToken))
                {
                    foreach (var val in page.Values)
                    {
                        if (val.ETag == null) val.ETag = new ETag("*");
                    }

                    await processor(page.Values);
                    continuationToken = page.ContinuationToken;
                    break;
                }
            } while (continuationToken != null);
        }

        private async Task DeleteAllEntitiesInBatchesAsync(
            TableClient table, Expression<Func<ITableEntity, bool>> filter)
        {
            Func<IEnumerable<ITableEntity>, Task> processor = async (entities) =>
            {
                if (_objectHistorySettings.AzureStorageConnectionString.Contains("=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw=="))
                {
                    foreach (var entity in entities)
                    {
                        await table.DeleteEntityAsync(entity.PartitionKey, entity.RowKey);
                    }
                }
                else
                {
                    var entityCount = entities.Count();
                    var loopCount = entityCount / 100 + entityCount % 100;
                    for (var i = 0; i < loopCount; i++)
                    {
                        var deleteEntitiesBatch = new List<TableTransactionAction>();
                        deleteEntitiesBatch.AddRange(entities.Skip(i*100).Take(100).Select(x => new TableTransactionAction(TableTransactionActionType.Delete, x)));
                        await table.SubmitTransactionAsync(deleteEntitiesBatch).ConfigureAwait(false);
                    }
                }
            };

            await ProcessEntitiesAsync(table, processor, filter);
        }

        public async Task DeleteObjectHistoryDeletedByPartitionKey(string partitionKey)
        {
            var cloudTable = GetObjectHistoryDeletedTable();
            await DeleteAllEntitiesInBatchesAsync(cloudTable, (table) => (table.PartitionKey == partitionKey));
        }

        public async Task<bool> HasObjectHistoryDeletedEntityAsync(string partitionKey)
        {
            var latest = await GetLatestObjectHistoryDeletedEntityAsync(partitionKey);
            return latest != null;
        }

        public async Task<bool> HasObjectHistoryEntityAsync(string partitionKey)
        {
            var latest = await GetLatestObjectHistoryEntityAsync(partitionKey);
            return latest != null;
        }
    }
}
