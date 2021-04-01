using System;

namespace Arragro.ObjectHistory.Core.Models
{
    public class ObjectHistoryGlobalEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        
        public int Version { get; set; }
        public string User { get; set; }
        public string ObjectName { get; set; }
        public Guid Folder { get; set; }
        public Guid? SubFolder { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        public ObjectHistoryGlobalEntity() { }

        public ObjectHistoryGlobalEntity(
            string partitionKey,
            string rowKey,
            int version,
            string user,
            string objectName,
            Guid folder,
            Guid? subFolder,
            DateTimeOffset timestamp)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
            Version = version;
            User = user;
            ObjectName = objectName;
            Folder = folder;
            SubFolder = subFolder;
            Timestamp = timestamp;
        }
    }
}
