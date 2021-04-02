using System;
using System.ComponentModel.DataAnnotations;

namespace Arragro.ObjectHistory.EFCore
{
    public class ObjectHistoryGlobalTableEntity
    {
        [MaxLength(1024)]
        public string PartitionKey { get; set; }
        [MaxLength(50)]
        public long RowKey { get; set; }
        public int Version { get; set; }
        public string User { get; set; }
        public string ObjectName { get; set; }
        public Guid Folder { get; set; }
        public Guid? SubFolder { get; set; }
        public bool IsAdd { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        [MaxLength(100)]
        public string Metadata { get; set; }
        /// <summary>
        /// Use this to help any validation you want to apply to the main controller I
        /// </summary>
        public string SecurityValidationToken { get; set; }

        public ObjectHistoryGlobalTableEntity() { }
        public ObjectHistoryGlobalTableEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = long.Parse(rowKey);
            Timestamp = DateTimeOffset.UtcNow;
        }
    }
}
