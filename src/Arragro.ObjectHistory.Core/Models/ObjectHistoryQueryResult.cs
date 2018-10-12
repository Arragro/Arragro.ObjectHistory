using Arragro.ObjectHistory.Core.Enums;
using System;

namespace Arragro.ObjectHistory.Core.Models
{
    public class ObjectHistoryQueryResult
    {
        public Guid Folder { get; set; }
        public string RowKey { get; set; }
        public string ApplicationName { get; set; }
        public string ObjectName { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public QueryResultType QueryResultType { get; set;}

        public ObjectHistoryQueryResult(string partitionKey, ObjectHistoryEntity objectHistoryEntity)
        {
            Folder = objectHistoryEntity.Folder;
            RowKey = objectHistoryEntity.RowKey;
            ObjectName = partitionKey;
            ApplicationName = objectHistoryEntity.ApplicationName;
            ModifiedBy = objectHistoryEntity.User;
            ModifiedDate = objectHistoryEntity.OriginTimestamp;
            QueryResultType = QueryResultType.Object;
        }
        
        public ObjectHistoryQueryResult(ObjectHistoryGlobalEntity objectHistoryGlobalEntity)
        {
            Folder = objectHistoryGlobalEntity.Folder;
            RowKey = objectHistoryGlobalEntity.RowKey;
            ObjectName = objectHistoryGlobalEntity.ObjectName;
            ModifiedBy = objectHistoryGlobalEntity.User;
            ModifiedDate = objectHistoryGlobalEntity.OriginTimestamp;
            QueryResultType = QueryResultType.Global;
        }
    }
}
