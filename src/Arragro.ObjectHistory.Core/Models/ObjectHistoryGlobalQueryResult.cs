using System;

namespace Arragro.ObjectHistory.Core.Models
{
    public class ObjectHistoryGlobalQueryResult
    {
        public Guid Folder { get; set; }
        public string RowKey { get; set; }
        public string ObjectName { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public ObjectHistoryGlobalQueryResult(ObjectHistoryGlobalEntity objectHistoryGlobalEntity)
        {
            Folder = objectHistoryGlobalEntity.Folder;
            RowKey = objectHistoryGlobalEntity.RowKey;
            ObjectName = objectHistoryGlobalEntity.ObjectName;
            ModifiedBy = objectHistoryGlobalEntity.User;
            ModifiedDate = objectHistoryGlobalEntity.OriginTimestamp;
           
        }
    }
}
