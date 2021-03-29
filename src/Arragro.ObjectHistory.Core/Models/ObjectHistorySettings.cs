namespace Arragro.ObjectHistory.Core.Models
{
    public class ObjectHistorySettings
    {
        public string AzureStorageConnectionString { get; set; }
        public string ApplicationName { get; set; }
        public string ObjectQueueName { get; set; } = "objectprocessor";
        public string ObjectInputContainerName { get; set; } = "objectprocessor";
        public string ObjectOutputContainerName { get; set; } = "trackedobjects";
        public string ObjectHistoryTable { get; set; } = "ObjectHistory";
        public string GlobalHistoryTable { get; set; } = "GlobalHistory";

        public ObjectHistorySettings()
        {
        }

        public ObjectHistorySettings(
            string azureStorageConnectionString,
            string applicationName)
        {
            AzureStorageConnectionString = azureStorageConnectionString;
            ApplicationName = applicationName;
        }
    }
}