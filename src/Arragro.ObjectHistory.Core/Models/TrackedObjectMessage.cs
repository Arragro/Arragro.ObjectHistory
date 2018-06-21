namespace Arragro.ObjectHistory.Core.Models
{
    public class TrackedObject
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string TimeStamp { get; set; }
        public string Folder { get; set; }
        public string NewJson { get; set; }
        public string OldJson { get; set; }

        public TrackedObject(string partitionKey, string rowKey, string timeStamp, string folder)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
            TimeStamp = timeStamp;
            Folder = folder;
        }
    }
}
