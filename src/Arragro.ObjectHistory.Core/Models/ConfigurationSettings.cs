namespace Arragro.ObjectHistory.Core.Models
{
    public class ObjectHistoryClientSettings
    {
        public string StorageConnectionString { get; set; }
        public string ObjectContainerName { get; set; }
        public string MessageQueueName { get; set; }
    }

    public class ConfigurationSettings
    {
        public ObjectHistoryClientSettings ObjectHistoryClientSettings { get; set; }      
    }
}
