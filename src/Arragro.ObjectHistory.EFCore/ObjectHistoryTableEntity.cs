using System;

namespace Arragro.ObjectHistory.EFCore
{
    public class ObjectHistoryTableEntity
    {
        public string PartitionKey { get; set; }
        public long RowKey { get; set; }
        public int Version { get; set; }
        public string ApplicationName { get; set; }
        public Guid Folder { get; set; }
        public Guid? SubFolder { get; set; }
        public string User { get; set; }
        public bool IsAdd { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        /// <summary>
        /// Use this to help any validation you want to apply to the main controller I
        /// </summary>
        public string SecurityValidationToken { get; set; }

        public ObjectHistoryTableEntity() { }

        public ObjectHistoryTableEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = long.Parse(rowKey);
            Timestamp = DateTimeOffset.UtcNow;
        }
    }
}
