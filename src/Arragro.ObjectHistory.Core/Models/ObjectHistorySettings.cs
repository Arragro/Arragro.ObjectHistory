namespace Arragro.ObjectHistory.Core.Models
{
    public class ObjectHistorySettings
    {
        public string StorageConnectionString { get; set; }
        public string ObjectContainerName { get; set; }
        public string MessageQueueName { get; set; }
        public string TableName { get; set; }
    }

}
