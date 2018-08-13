using System;
using System.Collections.Generic;
using System.Text;

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
        public ObjectHistoryQueryResult(ObjectHistoryEntity objectHistoryEntity)
        {
            Folder = objectHistoryEntity.Folder;
            RowKey = objectHistoryEntity.RowKey;
            ApplicationName = objectHistoryEntity.ApplicationName;
            ModifiedBy = objectHistoryEntity.User;
            ModifiedDate = objectHistoryEntity.OriginTimestamp;
        }
        public ObjectHistoryQueryResult(ObjectHistoryGlobalEntity objectHistoryGlobalEntity)
        {
            Folder = objectHistoryGlobalEntity.Folder;
            RowKey = objectHistoryGlobalEntity.RowKey;
            ObjectName = objectHistoryGlobalEntity.ObjectName;
            ModifiedBy = objectHistoryGlobalEntity.User;
            ModifiedDate = objectHistoryGlobalEntity.OriginTimestamp;
        }
    }
}
