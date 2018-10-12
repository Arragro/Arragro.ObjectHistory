using System;

namespace Arragro.ObjectHistory.Core.Models
{
    public class ObjectHistoryDetailRead : ObjectHistoryDetailBase
    {
        public ObjectHistoryDetailRead(string partitionKey, string rowKey, string applicationName, DateTime timeStamp, string user, Guid folder, bool isAdd = false)
            : base(partitionKey, rowKey, applicationName, timeStamp, user, folder, isAdd) { }

        public ObjectHistorySettings ObjectHistorySettings { get; set; }
        public object NewJson { get; set; }
        public object OldJson { get; set; }
        public object Diff { get; set; }

        public ObjectHistoryDetailRaw GetObjectHistoryDetailRaw()
        {
            try
            {
                if (this.IsAdd)
                {
                    return new ObjectHistoryDetailRaw(ObjectHistorySettings, PartitionKey, RowKey, ApplicationName, TimeStamp, User, Folder, SecurityValidationToken, IsAdd)
                    {
                        NewJson = NewJson.ToString(),
                        OldJson = null,
                        Diff = null
                    };
                }
                else
                {
                    return new ObjectHistoryDetailRaw(ObjectHistorySettings, PartitionKey, RowKey, ApplicationName, TimeStamp, User, Folder, SecurityValidationToken, IsAdd)
                    {
                        NewJson = NewJson.ToString(),
                        OldJson = this.OldJson.ToString(),
                        Diff = Diff.ToString()
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
