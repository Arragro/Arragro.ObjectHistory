using Microsoft.Azure.Cosmos.Table;
using System;

namespace Arragro.ObjectHistory.Core.Models
{
    public class ObjectHistoryGlobalEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }

        public string User { get; set; }
        public string ObjectName { get; set; }
        public Guid Folder { get; set; }
        public Guid? SubFolder { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        public ObjectHistoryGlobalEntity() { }
        public ObjectHistoryGlobalEntity(ObjectHistoryGlobalTableEntity objectHistoryGlobalTableEntity) 
        {
            PartitionKey = objectHistoryGlobalTableEntity.PartitionKey;
            RowKey = objectHistoryGlobalTableEntity.RowKey;
            User = objectHistoryGlobalTableEntity.User;
            ObjectName = objectHistoryGlobalTableEntity.ObjectName;
            Folder = objectHistoryGlobalTableEntity.Folder;
            SubFolder = objectHistoryGlobalTableEntity.SubFolder;
            Timestamp = objectHistoryGlobalTableEntity.Timestamp;
        }
    }

    public class ObjectHistoryGlobalTableEntity : TableEntity
    {
        public ObjectHistoryGlobalTableEntity() { }
        public ObjectHistoryGlobalTableEntity(string partitionKey, string rowKey) : base (partitionKey, rowKey)
        {
        }

        public string User { get; set; }
        public string ObjectName { get; set; }
        public Guid Folder { get; set; }
        public Guid? SubFolder { get; set; }
    }
}
