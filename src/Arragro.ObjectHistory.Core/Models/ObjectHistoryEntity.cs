using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Arragro.ObjectHistory.Core.Models
{
    public class ObjectHistoryEntity: TableEntity
    {
        public ObjectHistoryEntity() { }

        public ObjectHistoryEntity(string partitionKey, string rowKey)
        {
            this.PartitionKey = partitionKey;
            this.RowKey = rowKey;
        }

        public ObjectHistoryEntity(ObjectHistoryEntity objectHistoryEntity)
        {
            PartitionKey = objectHistoryEntity.PartitionKey;
            RowKey = objectHistoryEntity.RowKey;
            User = objectHistoryEntity.User;
            OriginTimestamp = objectHistoryEntity.OriginTimestamp;
            Folder = objectHistoryEntity.Folder;
            Timestamp = objectHistoryEntity.Timestamp;
            ETag = objectHistoryEntity.ETag;
        }

        public string ApplicationName { get; set; }
        public Guid Folder { get; set; }
        public DateTime OriginTimestamp { get; set; }
        public string User { get; set; }
    }
}
