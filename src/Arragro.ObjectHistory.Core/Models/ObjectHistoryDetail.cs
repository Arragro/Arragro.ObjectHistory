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
        public Guid? SubFolder { get; set; }
        public string User { get; set; }
        public bool IsAdd { get; set; }
        /// <summary>
        /// Use this to help any validation you want to apply to the main controller I
        /// </summary>
        public string SecurityValidationToken { get; set; }

        public ObjectHistoryDetailBase(string partitionKey, string rowKey, string applicationName, DateTime timeStamp, string user, Guid folder, bool isAdd = false, Guid? subFolder = null, string securityValidationToken = null)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
            ApplicationName = applicationName;
            TimeStamp = timeStamp;
            User = user;
            Folder = folder;
            SubFolder = subFolder;
            IsAdd = isAdd;
            SecurityValidationToken = securityValidationToken;
        }
    }
}