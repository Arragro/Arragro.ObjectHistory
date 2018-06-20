namespace ConsoleApp.ObjectHistoryClient.Models
{
    public class TrackedObjectMessage
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string TimeStamp { get; set; }
        public string Folder { get; set; }

        public TrackedObjectMessage(string partitionKey, string rowKey, string timeStamp, string folder)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
            TimeStamp = timeStamp;
            Folder = folder;
        }
    }
}
