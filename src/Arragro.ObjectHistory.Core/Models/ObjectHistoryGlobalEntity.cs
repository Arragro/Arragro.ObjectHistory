using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Arragro.ObjectHistory.Core.Models
{
    public class ObjectHistoryGlobalEntity : TableEntity
    {
        public ObjectHistoryGlobalEntity() { }
        public ObjectHistoryGlobalEntity(string partitionKey, string rowKey)
        {
            this.PartitionKey = partitionKey;
            this.RowKey = rowKey;
        }

        public string User { get; set; }
        public string ObjectName { get; set; }
        public Guid Folder { get; set; }
        public DateTime OriginTimestamp { get; set; }

    }
}
