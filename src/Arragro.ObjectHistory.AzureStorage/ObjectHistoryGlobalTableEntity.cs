using Microsoft.Azure.Cosmos.Table;
using System;

namespace Arragro.ObjectHistory.AzureStorage
{
    public class ObjectHistoryGlobalTableEntity : TableEntity
    {
        public string User { get; set; }
        public string ObjectName { get; set; }
        public Guid Folder { get; set; }
        public Guid? SubFolder { get; set; }
        public bool IsAdd { get; set; }
        /// <summary>
        /// Use this to help any validation you want to apply to the main controller I
        /// </summary>
        public string SecurityValidationToken { get; set; }

        public ObjectHistoryGlobalTableEntity() { }
        public ObjectHistoryGlobalTableEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
        }
    }
}
