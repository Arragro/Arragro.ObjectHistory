using Arragro.ObjectHistory.Core.Enums;
using System;

namespace Arragro.ObjectHistory.Core.Models
{
    public class ObjectHistoryQueryResult
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public int Version { get; set; }
        public Guid Folder { get; set; }
        public string ApplicationName { get; set; }
        public string ModifiedBy { get; set; }
        public DateTimeOffset ModifiedDate { get; set; }
        public string Metadata { get; set; }
        public QueryResultType QueryResultType { get; set;}

        public ObjectHistoryQueryResult(ObjectHistoryEntity objectHistoryEntity)
        {
            Folder = objectHistoryEntity.Folder;
            RowKey = objectHistoryEntity.RowKey;
            Version = objectHistoryEntity.Version;
            PartitionKey = objectHistoryEntity.PartitionKey;
            ApplicationName = objectHistoryEntity.ApplicationName;
            ModifiedBy = objectHistoryEntity.User;
            ModifiedDate = objectHistoryEntity.Timestamp;
            Metadata = objectHistoryEntity.Metadata;
            QueryResultType = QueryResultType.Object;
        }

        public ObjectHistoryQueryResult(ObjectHistoryDeletedEntity objectHistoryEntity)
        {
            Folder = objectHistoryEntity.Folder;
            RowKey = objectHistoryEntity.RowKey;
            Version = objectHistoryEntity.Version;
            PartitionKey = objectHistoryEntity.PartitionKey;
            ApplicationName = objectHistoryEntity.ApplicationName;
            ModifiedBy = objectHistoryEntity.User;
            ModifiedDate = objectHistoryEntity.Timestamp;
            Metadata = objectHistoryEntity.Metadata;
            QueryResultType = QueryResultType.Deleted;
        }

        public ObjectHistoryQueryResult(ObjectHistoryGlobalEntity objectHistoryGlobalEntity)
        {
            Folder = objectHistoryGlobalEntity.Folder;
            RowKey = objectHistoryGlobalEntity.RowKey;
            Version = objectHistoryGlobalEntity.Version;
            PartitionKey = objectHistoryGlobalEntity.ObjectName;
            ModifiedBy = objectHistoryGlobalEntity.User;
            ModifiedDate = objectHistoryGlobalEntity.Timestamp;
            Metadata = objectHistoryGlobalEntity.Metadata;
            QueryResultType = QueryResultType.Global;
        }
    }
}
