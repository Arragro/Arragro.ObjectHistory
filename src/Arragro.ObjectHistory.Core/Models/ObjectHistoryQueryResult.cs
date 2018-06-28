using System;

namespace Arragro.ObjectHistory.Core.Models
{
    public class ObjectHistoryQueryResult
    {
        public Guid Folder { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedDate { get; set; }
        public ObjectHistoryQueryResult(ObjectHistoryEntity objectHistoryEntity)
        {
            Folder = objectHistoryEntity.Folder;
            ModifiedBy = objectHistoryEntity.User;
            ModifiedDate = objectHistoryEntity.RowKey;
        }
    }
}
