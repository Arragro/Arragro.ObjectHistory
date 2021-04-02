using Microsoft.Azure.Cosmos.Table;
using System;

namespace Arragro.ObjectHistory.AzureStorage
{
    public class ObjectHistoryDeletedTableEntity : TableEntity
    {
        public int Verion { get; set; }
        public string ApplicationName { get; set; }
        public Guid Folder { get; set; }
        public Guid? SubFolder { get; set; }
        public string User { get; set; }
        public string Metadata { get; set; }
        /// <summary>
        /// Use this to help any validation you want to apply to the main controller I
        /// </summary>
        public string SecurityValidationToken { get; set; }

        public ObjectHistoryDeletedTableEntity() { }

        public ObjectHistoryDeletedTableEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
        }
    }
}
