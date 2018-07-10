using System;

namespace Arragro.ObjectHistory.Core.Models
{
    public class ObjectHistoryQueryResult
    {
        public Guid Folder { get; set; }
        public string RowKey { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public ObjectHistoryQueryResult(ObjectHistoryEntity objectHistoryEntity)
        {
            Folder = objectHistoryEntity.Folder;
            RowKey = objectHistoryEntity.RowKey;
            ModifiedBy = objectHistoryEntity.User;
            ModifiedDate = objectHistoryEntity.OriginTimestamp;
            
        }
    }
}
