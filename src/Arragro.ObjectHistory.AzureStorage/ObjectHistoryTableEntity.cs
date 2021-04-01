using Microsoft.Azure.Cosmos.Table;
using System;

namespace Arragro.ObjectHistory.AzureStorage
{
    public class ObjectHistoryTableEntity : TableEntity
    {
        public int Verion { get; set; }
        public string ApplicationName { get; set; }
        public Guid Folder { get; set; }
        public Guid? SubFolder { get; set; }
        public string User { get; set; }
        public bool IsAdd { get; set; }
        /// <summary>
        /// Use this to help any validation you want to apply to the main controller I
        /// </summary>
        public string SecurityValidationToken { get; set; }

        public ObjectHistoryTableEntity() { }

        public ObjectHistoryTableEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
        }
    }
}
