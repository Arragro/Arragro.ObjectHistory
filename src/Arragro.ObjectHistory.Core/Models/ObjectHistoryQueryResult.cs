using Arragro.ObjectHistory.Core.Enums;
using System;

namespace Arragro.ObjectHistory.Core.Models
{
    public class ObjectHistoryQueryResult
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public Guid Folder { get; set; }
        public string ApplicationName { get; set; }
        public string ModifiedBy { get; set; }
        public DateTimeOffset ModifiedDate { get; set; }
        public QueryResultType QueryResultType { get; set;}

        public ObjectHistoryQueryResult(ObjectHistoryEntity objectHistoryEntity)
        {
            Folder = objectHistoryEntity.Folder;
            RowKey = objectHistoryEntity.RowKey;
            PartitionKey = objectHistoryEntity.PartitionKey;
            ApplicationName = objectHistoryEntity.ApplicationName;
            ModifiedBy = objectHistoryEntity.User;
            QueryResultType = QueryResultType.Object;
        }
        
        public ObjectHistoryQueryResult(ObjectHistoryGlobalEntity objectHistoryGlobalEntity)
        {
            Folder = objectHistoryGlobalEntity.Folder;
            RowKey = objectHistoryGlobalEntity.RowKey;
            PartitionKey = objectHistoryGlobalEntity.ObjectName;
            ModifiedBy = objectHistoryGlobalEntity.User;
            ModifiedDate = objectHistoryGlobalEntity.Timestamp;
            QueryResultType = QueryResultType.Global;
        }
    }
}
