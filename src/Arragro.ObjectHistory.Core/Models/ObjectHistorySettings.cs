namespace Arragro.ObjectHistory.Core.Models
{
    public class ObjectHistorySettings
    {
        public string ApplicationName { get; set; }
        public string StorageConnectionString { get; set; }
        public string ObjectContainerName { get; set; }
        public string MessageQueueName { get; set; }
        public string ObjectHistoryTable { get; set; }
        public string GlobalHistoryTable { get; set; }
    }

}
