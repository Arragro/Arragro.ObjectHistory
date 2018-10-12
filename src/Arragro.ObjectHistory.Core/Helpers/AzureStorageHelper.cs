using Arragro.ObjectHistory.Core.Models;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Arragro.ObjectHistory.Core.Helpers
{
    public class AzureStorageHelper
    {
        public async Task UploadJsonFileAsync(CloudBlobContainer objectContainer, Guid folder, string fileName, string objectHistoryJson)
        {
            try
            {
                var options = new BlobRequestOptions()
                {
                    ServerTimeout = TimeSpan.FromMinutes(10)
                };

                CloudBlockBlob cloudBlockBlob = objectContainer.GetBlockBlobReference($"{folder}/{fileName}");

                using (var ms = new MemoryStream())
                {
                    LoadStreamWithJson(ms, objectHistoryJson);
                    await cloudBlockBlob.UploadFromStreamAsync(ms);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void LoadStreamWithJson(Stream ms, string file)
        {
            StreamWriter writer = new StreamWriter(ms);
            writer.Write(file);
            writer.Flush();
            ms.Position = 0;
        }

        public async Task SendQueueMessage(CloudQueue queue, string message)
        {
            try
            {
                var cloudQueueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(new ObjectHistoryMessge { Message = message }));
                await queue.AddMessageAsync(cloudQueueMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task AddObjectHistoryEntityRecordAsync(ObjectHistoryDetailBase objectHistoryDetails, CloudTable table)
        {
            try
            {
                var objectHistoryEntity = new ObjectHistoryEntity(objectHistoryDetails.PartitionKey, objectHistoryDetails.RowKey)
                {
                    ApplicationName = objectHistoryDetails.ApplicationName,
                    OriginTimestamp = objectHistoryDetails.TimeStamp,
                    User = objectHistoryDetails.User,
                    Folder = objectHistoryDetails.Folder
                };

                var insertOperation = TableOperation.Insert(objectHistoryEntity);
                await table.ExecuteAsync(insertOperation);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Somthing has gone wrong with the adding of the table record. Please review the inner exception. {0}", ex.InnerException));
            }
        }

        public async Task AddObjectHistoryGlobalAsync(ObjectHistoryDetailBase objectHistoryDetails, CloudTable table)
        {
            try
            {
                var objectHistoryEntity = new ObjectHistoryGlobalEntity(objectHistoryDetails.ApplicationName, objectHistoryDetails.RowKey)
                {
                    OriginTimestamp = objectHistoryDetails.TimeStamp,
                    User = objectHistoryDetails.User,
                    ObjectName = objectHistoryDetails.PartitionKey,
                    Folder = objectHistoryDetails.Folder
                };

                var insertOperation = TableOperation.Insert(objectHistoryEntity);
                await table.ExecuteAsync(insertOperation);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Somthing has gone wrong with the adding of the table record. Please review the inner exception. {0}", ex.InnerException));
            }
        }

        public async Task<ObjectHistoryEntity> GetLastObjectHistoryEntity(string partitionKey, CloudTable table)
        {
            var query = new TableQuery<ObjectHistoryEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));

            return (await table.ExecuteQuerySegmentedAsync(query.Take(1), null)).SingleOrDefault();
        }

        public async Task<Guid?> GetLatestBlobFolderNameByPartitionKeyAsync(string partitionKey, CloudTable table)
        {
            try
            {
                var objectHistoryEntity = await GetLastObjectHistoryEntity(partitionKey, table);
                if (objectHistoryEntity == null)
                    return null;

                return objectHistoryEntity.Folder;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ObjectHistoryEntity> GetObjectHistoryRecord(string partitionKey, string rowKey, CloudTable table)
        {
            try
            {
                var retrieveOperation = TableOperation.Retrieve<ObjectHistoryEntity>(partitionKey, rowKey);

                var retrievedResult = await table.ExecuteAsync(retrieveOperation);

                return retrievedResult.Result as ObjectHistoryEntity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ObjectHistoryQueryResultContainer> GetObjectHistoryRecordsByObjectNamePartitionKey(string partitionKey, CloudTable table, TableContinuationToken token)
        {
            try
            {
                var query = new TableQuery<ObjectHistoryEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));

                var queryResult = await table.ExecuteQuerySegmentedAsync(query.Take(10), token);
                token = queryResult.ContinuationToken;

                var entityResults = new ObjectHistoryQueryResultContainer(queryResult.Results, token, partitionKey);

                return entityResults;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ObjectHistoryQueryResultContainer> GetObjectHistoryRecordsByApplicationNamePartitionKey(string partitionKey, CloudTable table, TableContinuationToken token)
        {
            try
            {
                var query = new TableQuery<ObjectHistoryGlobalEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));

                var queryResult = await table.ExecuteQuerySegmentedAsync(query.Take(10), token);
                token = queryResult.ContinuationToken;

                var entityResults = new ObjectHistoryQueryResultContainer(queryResult.Results, token, partitionKey);

                return entityResults;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task<string> DownloadBlob(CloudBlobContainer objectContainer, string folder, string filename)
        {
            try
            {
                var options = new BlobRequestOptions()
                {
                    ServerTimeout = TimeSpan.FromMinutes(10)
                };

                CloudBlockBlob cloudBlockBlob = objectContainer.GetBlockBlobReference($"{folder}/{filename}");

                return cloudBlockBlob.DownloadTextAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
