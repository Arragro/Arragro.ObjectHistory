using Microsoft.WindowsAzure.Storage.Table;

namespace Arragro.ObjectHistory.Core.Models
{
    public class ObjectHistoryDetails
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string TimeStamp { get; set; }
        public string Folder { get; set; }
        public string User { get; set; }
        public string NewJson { get; set; }
        public string OldJson { get; set; }
        public string Diff { get; set; }

        public ObjectHistoryDetails(string partitionKey, string rowKey, string timeStamp, string user, string folder)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
            TimeStamp = timeStamp;
            User = user;
            Folder = folder;
        }
    }
}
