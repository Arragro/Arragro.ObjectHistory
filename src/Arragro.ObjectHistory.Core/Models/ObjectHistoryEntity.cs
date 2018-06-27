using Microsoft.WindowsAzure.Storage.Table;

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

        public string Folder { get; set; }
        public string OriginTimestamp { get; set; }
        public string User { get; set; }
    }
}
