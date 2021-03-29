using Microsoft.Azure.Cosmos.Table;
using System;

namespace Arragro.ObjectHistory.Core.Models
{
    public class ObjectHistoryEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }

        public string ApplicationName { get; set; }
        public Guid Folder { get; set; }
        public Guid? SubFolder { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string User { get; set; }
        public string IsAdd { get; set; }
        /// <summary>
        /// Use this to help any validation you want to apply to the main controller I
        /// </summary>
        public string SecurityValidationToken { get; set; }

        public ObjectHistoryEntity() { }

        public ObjectHistoryEntity(ObjectHistoryTableEntity objectHistoryTableEntity)
        {
            PartitionKey = objectHistoryTableEntity.PartitionKey;
            RowKey = objectHistoryTableEntity.RowKey;
            ApplicationName = objectHistoryTableEntity.ApplicationName;
            Folder = objectHistoryTableEntity.Folder;
            SubFolder = objectHistoryTableEntity.SubFolder;
            Timestamp = objectHistoryTableEntity.Timestamp;
            User = objectHistoryTableEntity.User;
            IsAdd = objectHistoryTableEntity.IsAdd;
            SecurityValidationToken = objectHistoryTableEntity.SecurityValidationToken;
        }
    }

    public class ObjectHistoryTableEntity : TableEntity
    {
        public string ApplicationName { get; set; }
        public Guid Folder { get; set; }
        public Guid? SubFolder { get; set; }
        public string User { get; set; }
        public string IsAdd { get; set; }
        /// <summary>
        /// Use this to help any validation you want to apply to the main controller I
        /// </summary>
        public string SecurityValidationToken { get; set; }

        public ObjectHistoryTableEntity() { }

        public ObjectHistoryTableEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
        }
    }
}
