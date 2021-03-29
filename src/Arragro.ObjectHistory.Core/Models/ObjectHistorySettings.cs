namespace Arragro.ObjectHistory.Core.Models
{
    public class ObjectHistorySettingsBase
    {
        public string ObjectQueueName { get; set; } = "objectprocessor";
        public string ObjectInputContainerName { get; set; } = "objectprocessor";
        public string ObjectOutputContainerName { get; set; } = "trackedobjects";
        public string ObjectHistoryTable { get; set; } = "ObjectHistory";
        public string GlobalHistoryTable { get; set; } = "GlobalHistory";
    }

    public class ObjectHistorySettings : ObjectHistorySettingsBase
    {
        public string AzureStorageConnectionString { get; set; }
        public string ApplicationName { get; set; }

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

        public ObjectHistorySettingsBase ToObjectHistorySettingsBase()
        {
            return new ObjectHistorySettingsBase
            {
                ObjectQueueName = this.ObjectQueueName,
                GlobalHistoryTable = this.GlobalHistoryTable,
                ObjectHistoryTable = this.ObjectHistoryTable,
                ObjectInputContainerName = this.ObjectInputContainerName,
                ObjectOutputContainerName = this.ObjectOutputContainerName
            };
        }
    }
}