using Arragro.ObjectHistory.Core.Helpers;
using Newtonsoft.Json;
using System;

namespace Arragro.ObjectHistory.Core.Models
{
    public class ObjectHistoryDetailBase
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string ApplicationName { get; set; }
        public DateTime TimeStamp { get; set; }
        public Guid Folder { get; set; }
        public string User { get; set; }
        public bool IsAdd { get; set; }

        public ObjectHistoryDetailBase(string partitionKey, string rowKey, string applicationName, DateTime timeStamp, string user, Guid folder, bool isAdd = false)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
            ApplicationName = applicationName;
            TimeStamp = timeStamp;
            User = user;
            Folder = folder;
            IsAdd = isAdd;
        }
    }
}