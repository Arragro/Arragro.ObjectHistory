using Arragro.ObjectHistory.Core.Helpers;
using Arragro.ObjectHistory.Core.Interfaces;
using Arragro.ObjectHistory.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Arragro.ObjectHistory.EFCore
{
    public class EFStorageHelper : QueueAndBlobStorageHelper, IStorageHelper
    {
        private readonly ArragroObjectHistoryBaseContext _arragroObjectHistoryBaseContext;

        public EFStorageHelper(
            ObjectHistorySettings objectHistorySettings,
            ArragroObjectHistoryBaseContext arragroObjectHistoryBaseContext) : base(objectHistorySettings)
        {
            _arragroObjectHistoryBaseContext = arragroObjectHistoryBaseContext;
        }

        public async Task AddObjectHistoryEntityRecordAsync(ObjectHistoryDetailBase objectHistoryDetails)
        {
            try
            {
                var objectHistoryEntity = new ObjectHistoryTableEntity(objectHistoryDetails.PartitionKey, objectHistoryDetails.RowKey)
                {
                    ApplicationName = objectHistoryDetails.ApplicationName,
                    User = objectHistoryDetails.User,
                    Folder = objectHistoryDetails.Folder,
                    SubFolder = objectHistoryDetails.SubFolder,
                    IsAdd = objectHistoryDetails.IsAdd,
                    SecurityValidationToken = objectHistoryDetails.SecurityValidationToken
                };

                if (_arragroObjectHistoryBaseContext.ObjectHistoryTableEntity.Any(x => 
                        x.PartitionKey == objectHistoryEntity.PartitionKey &&
                        x.RowKey == objectHistoryEntity.RowKey))
                    _arragroObjectHistoryBaseContext.ObjectHistoryTableEntity.Update(objectHistoryEntity);
                else
                    _arragroObjectHistoryBaseContext.ObjectHistoryTableEntity.Add(objectHistoryEntity);
                await _arragroObjectHistoryBaseContext.SaveChangesAsync();
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
                var objectHistoryEntity = new ObjectHistoryDeletedTableEntity(objectHistoryDetails.PartitionKey, objectHistoryDetails.RowKey)
                {
                    ApplicationName = objectHistoryDetails.ApplicationName,
                    User = objectHistoryDetails.User,
                    Folder = objectHistoryDetails.Folder,
                    SubFolder = objectHistoryDetails.SubFolder,
                    SecurityValidationToken = objectHistoryDetails.SecurityValidationToken
                };

                if (_arragroObjectHistoryBaseContext.ObjectHistoryDeletedTableEntities.Any(x =>
                        x.PartitionKey == objectHistoryEntity.PartitionKey &&
                        x.RowKey == objectHistoryEntity.RowKey))
                    _arragroObjectHistoryBaseContext.ObjectHistoryDeletedTableEntities.Update(objectHistoryEntity);
                else
                    _arragroObjectHistoryBaseContext.ObjectHistoryDeletedTableEntities.Add(objectHistoryEntity);
                await _arragroObjectHistoryBaseContext.SaveChangesAsync();
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
                var objectHistoryEntity = new ObjectHistoryGlobalTableEntity(objectHistoryDetails.ApplicationName, objectHistoryDetails.RowKey)
                {
                    User = objectHistoryDetails.User,
                    ObjectName = objectHistoryDetails.PartitionKey,
                    Folder = objectHistoryDetails.Folder,
                    SubFolder = objectHistoryDetails.SubFolder,
                    IsAdd = objectHistoryDetails.IsAdd,
                    SecurityValidationToken = objectHistoryDetails.SecurityValidationToken
                };

                if (_arragroObjectHistoryBaseContext.ObjectHistoryGlobalTableEntity.Any(x =>
                        x.PartitionKey == objectHistoryEntity.PartitionKey &&
                        x.RowKey == objectHistoryEntity.RowKey))
                    _arragroObjectHistoryBaseContext.ObjectHistoryGlobalTableEntity.Update(objectHistoryEntity);
                else
                    _arragroObjectHistoryBaseContext.ObjectHistoryGlobalTableEntity.Add(objectHistoryEntity);
                await _arragroObjectHistoryBaseContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Somthing has gone wrong with the adding of the table record. Please review the inner exception. {0}", ex.InnerException));
            }
        }

        public async Task<ObjectHistoryEntity> GetLastObjectHistoryEntityAsync(string partitionKey)
        {
            var objectHistoryTableEntity = await _arragroObjectHistoryBaseContext.ObjectHistoryTableEntity.Where(x => x.PartitionKey == partitionKey).OrderBy(x => x.RowKey).Take(1).SingleOrDefaultAsync();
            if (objectHistoryTableEntity == null)
                return null;
            return new ObjectHistoryEntity(
                objectHistoryTableEntity.PartitionKey,
                objectHistoryTableEntity.RowKey.ToString(),
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
            var objectHistoryTableEntity = await _arragroObjectHistoryBaseContext.ObjectHistoryDeletedTableEntities.Where(x => x.PartitionKey == partitionKey).OrderBy(x => x.RowKey).Take(1).SingleOrDefaultAsync();
            if (objectHistoryTableEntity == null)
                return null;
            return new ObjectHistoryDeletedEntity(
                objectHistoryTableEntity.PartitionKey,
                objectHistoryTableEntity.RowKey.ToString(),
                objectHistoryTableEntity.ApplicationName,
                objectHistoryTableEntity.Folder,
                objectHistoryTableEntity.SubFolder,
                objectHistoryTableEntity.Timestamp,
                objectHistoryTableEntity.User,
                objectHistoryTableEntity.SecurityValidationToken);
        }

        public async Task<Guid?> GetLatestBlobFolderNameByPartitionKeyAsync(string partitionKey)
        {
            var objectHistoryEntity = await GetLastObjectHistoryEntityAsync(partitionKey);
            if (objectHistoryEntity == null)
                return null;

            return objectHistoryEntity.Folder;
        }

        public async Task<ObjectHistoryEntity> GetObjectHistoryRecordAsync(string partitionKey, string rowKey)
        {
            var objectHistoryTableEntity = await _arragroObjectHistoryBaseContext.ObjectHistoryTableEntity.Where(x => x.PartitionKey == partitionKey && x.RowKey == long.Parse(rowKey)).SingleAsync();

            return new ObjectHistoryEntity(
                objectHistoryTableEntity.PartitionKey,
                objectHistoryTableEntity.RowKey.ToString(),
                objectHistoryTableEntity.ApplicationName,
                objectHistoryTableEntity.Folder,
                objectHistoryTableEntity.SubFolder,
                objectHistoryTableEntity.Timestamp,
                objectHistoryTableEntity.User,
                objectHistoryTableEntity.IsAdd,
                objectHistoryTableEntity.SecurityValidationToken);
        }

        public async Task<ObjectHistoryQueryResultContainer> GetObjectHistoryDeletedRecordsAsync(PagingToken pagingToken)
        {
            var objectHistoryTableEntity = await _arragroObjectHistoryBaseContext.ObjectHistoryDeletedTableEntities.Skip((pagingToken.Page - 1) * pagingToken.PageSize).Take(pagingToken.PageSize).ToListAsync();

            var entityResults = new ObjectHistoryQueryResultContainer(objectHistoryTableEntity.Select(objectHistoryTableEntity =>
                new ObjectHistoryDeletedEntity(
                    objectHistoryTableEntity.PartitionKey,
                    objectHistoryTableEntity.RowKey.ToString(),
                    objectHistoryTableEntity.ApplicationName,
                    objectHistoryTableEntity.Folder,
                    objectHistoryTableEntity.SubFolder,
                    objectHistoryTableEntity.Timestamp,
                    objectHistoryTableEntity.User,
                    objectHistoryTableEntity.SecurityValidationToken)),
                pagingToken);

            return entityResults;
        }

        public async Task<ObjectHistoryQueryResultContainer> GetObjectHistoryRecordsByObjectNamePartitionKeyAsync(string partitionKey, PagingToken pagingToken)
        {
            var objectHistoryTableEntity = await _arragroObjectHistoryBaseContext.ObjectHistoryTableEntity
                                                    .Where(x => x.PartitionKey == partitionKey)
                                                    .OrderBy(x => x.RowKey)
                                                    .Skip((pagingToken.Page - 1) * pagingToken.PageSize)
                                                    .Take(pagingToken.PageSize)
                                                    .ToListAsync();

            var entityResults = new ObjectHistoryQueryResultContainer(objectHistoryTableEntity.Select(objectHistoryTableEntity =>
                new ObjectHistoryEntity(
                    objectHistoryTableEntity.PartitionKey,
                    objectHistoryTableEntity.RowKey.ToString(),
                    objectHistoryTableEntity.ApplicationName,
                    objectHistoryTableEntity.Folder,
                    objectHistoryTableEntity.SubFolder,
                    objectHistoryTableEntity.Timestamp,
                    objectHistoryTableEntity.User,
                    objectHistoryTableEntity.IsAdd,
                    objectHistoryTableEntity.SecurityValidationToken)),
                pagingToken, partitionKey);

            return entityResults;
        }

        public async Task<ObjectHistoryQueryResultContainer> GetObjectHistoryRecordsByApplicationNamePartitionKeyAsync(string partitionKey, PagingToken pagingToken)
        {
            var objectHistoryTableEntity = await _arragroObjectHistoryBaseContext.ObjectHistoryGlobalTableEntity
                                                    .Where(x => x.PartitionKey == partitionKey)
                                                    .OrderBy(x => x.RowKey)
                                                    .Skip((pagingToken.Page - 1) * pagingToken.PageSize)
                                                    .Take(pagingToken.PageSize)
                                                    .ToListAsync();

            var entityResults = new ObjectHistoryQueryResultContainer(objectHistoryTableEntity.Select(objectHistoryGlobalEntity =>
                new ObjectHistoryGlobalEntity(
                    objectHistoryGlobalEntity.PartitionKey,
                    objectHistoryGlobalEntity.RowKey.ToString(),
                    objectHistoryGlobalEntity.User,
                    objectHistoryGlobalEntity.ObjectName,
                    objectHistoryGlobalEntity.Folder,
                    objectHistoryGlobalEntity.SubFolder,
                    objectHistoryGlobalEntity.Timestamp)),
                pagingToken, partitionKey);

            return entityResults;
        }

        public async Task DeleteObjectHistoryDeletedByPartitionKey(string partitionKey)
        {
            var objectHistoriesDeleted = await _arragroObjectHistoryBaseContext.ObjectHistoryDeletedTableEntities.Where(x => x.PartitionKey == partitionKey).ToListAsync();
            _arragroObjectHistoryBaseContext.ObjectHistoryDeletedTableEntities.RemoveRange(objectHistoriesDeleted);
            await _arragroObjectHistoryBaseContext.SaveChangesAsync();
        }
    }
}
