using System;

namespace Arragro.ObjectHistory.Core.Models
{
    public enum StorageType
    {
        AzureStorage,
        Postgres,
        SqlServer,
        Sqlite
    }

    public class ObjectHistorySettingsBase
    {
        public string ObjectQueueName { get; set; } = "objectprocessor";
        public string ObjectInputContainerName { get; set; } = "objectprocessor";
        public string ObjectOutputContainerName { get; set; } = "trackedobjects";
        public string ObjectHistoryTable { get; set; } = "ObjectHistory";
        public string ObjectHistoryDeletedTable { get; set; } = "ObjectDeletedHistory";
        public string GlobalHistoryTable { get; set; } = "GlobalHistory";
    }

    public class ObjectHistorySettings : ObjectHistorySettingsBase
    {
        public string AzureStorageConnectionString { get; set; }
        public string DatabaseConnectionString { get; set; }
        public string ApplicationName { get; set; }
        public StorageType StorageType { get; set; } = StorageType.AzureStorage;

        public ObjectHistorySettings()
        {
        }

        public ObjectHistorySettings(
            string azureStorageConnectionString,
            string databaseConnectionString,
            string applicationName,
            StorageType storageType)
        {
            if (string.IsNullOrEmpty(azureStorageConnectionString))
                throw new ArgumentNullException(nameof(azureStorageConnectionString));
            if (string.IsNullOrEmpty(databaseConnectionString))
                throw new ArgumentNullException(nameof(databaseConnectionString));

            AzureStorageConnectionString = azureStorageConnectionString;
            DatabaseConnectionString = databaseConnectionString;
            ApplicationName = applicationName;
            StorageType = storageType;
        }

        public ObjectHistorySettings(
            string azureStorageConnectionString,
            string applicationName)
        {
            AzureStorageConnectionString = azureStorageConnectionString;
            ApplicationName = applicationName;
            StorageType = StorageType.AzureStorage;
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