using System;

namespace Arragro.ObjectHistory.Core.Models
{
    public class ObjectHistoryDeletedEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }

        public int Version { get; set; }
        public string ApplicationName { get; set; }
        public Guid Folder { get; set; }
        public Guid? SubFolder { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string User { get; set; }
        public string Metadata { get; set; }
        /// <summary>
        /// Use this to help any validation you want to apply to the main controller I
        /// </summary>
        public string SecurityValidationToken { get; set; }

        public ObjectHistoryDeletedEntity() { }

        public ObjectHistoryDeletedEntity(
            string partitionKey,
            string rowKey,
            int version,
            string applicationName,
            Guid folder,
            Guid? subFolder,
            DateTimeOffset timestamp,
            string user,
            string metadata,
            string securityValidationToken)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
            Version = version;
            ApplicationName = applicationName;
            Folder = folder;
            SubFolder = subFolder;
            Timestamp = timestamp;
            User = user;
            Metadata = metadata;
            SecurityValidationToken = securityValidationToken;
        }

        public string GetBlobPath()
        {
            return SubFolder.HasValue ? $"{Folder}/{SubFolder}" : $"{Folder}";
        }
    }
}
