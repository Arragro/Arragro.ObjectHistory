using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Arragro.ObjectHistory.Core.Models
{
    public class ObjectHistoryDetail
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string ApplicationName { get; set; }
        public DateTime TimeStamp { get; set; }
        public Guid Folder { get; set; }
        public string User { get; set; }
        public string NewJson { get; set; }
        public string OldJson { get; set; }
        public string Diff { get; set; }

        public ObjectHistoryDetail(string partitionKey, string rowKey, string applicationName, DateTime timeStamp, string user, Guid folder)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
            ApplicationName = applicationName;
            TimeStamp = timeStamp;
            User = user;
            Folder = folder;
        }
    }
}
