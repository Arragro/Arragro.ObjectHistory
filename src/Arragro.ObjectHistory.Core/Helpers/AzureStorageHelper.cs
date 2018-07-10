using Arragro.ObjectHistory.Core.Models;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
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

                var cloudQueueMessage = new CloudQueueMessage(message);
                await queue.AddMessageAsync(cloudQueueMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task AddObjectHistoryEntityRecord(ObjectHistoryDetail objectHistoryDetails, CloudTable table)
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

        public async Task AddObjectHistoryGlobal(ObjectHistoryDetail objectHistoryDetails, CloudTable table)
        {
            try
            {
                var objectHistoryEntity = new ObjectHistoryGlobalEntity(objectHistoryDetails.ApplicationName, objectHistoryDetails.RowKey)
                {
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
        public async Task<Guid> GetLatestBlobFolderNameByPartitionKey(string partitionKey, CloudTable table)
        {
            try
            {
                var query = new TableQuery<ObjectHistoryEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));

                var queryResult = await table.ExecuteQuerySegmentedAsync(query.Take(1), null);

                var folder = (queryResult.Results).Select(x => x.Folder).FirstOrDefault();

                return folder;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<ObjectHistoryQueryResultContainer> GetObjectHistoryRecordsByPartitionKey(string partitionKey, CloudTable table, TableContinuationToken token)
        {
            try
            {
                var query = new TableQuery<ObjectHistoryEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));

                var queryResult = await table.ExecuteQuerySegmentedAsync(query.Take(1), token);
                token = queryResult.ContinuationToken;

                var entityResults = new ObjectHistoryQueryResultContainer(queryResult.Results, token, partitionKey);

                return entityResults;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


    }
}
