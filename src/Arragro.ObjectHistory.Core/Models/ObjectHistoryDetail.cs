using Arragro.ObjectHistory.Core.Helpers;
using Newtonsoft.Json;
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
        public string User { get; set; }
        public bool IsAdd { get; set; }

        public ObjectHistoryDetailBase(string partitionKey, string rowKey, string applicationName, DateTime timeStamp, string user, Guid folder, bool isAdd = false)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
            ApplicationName = applicationName;
            TimeStamp = timeStamp;
            User = user;
            Folder = folder;
            IsAdd = isAdd;
        }
    }

    public class ObjectHistoryDetailRaw : ObjectHistoryDetailBase
    {
        public ObjectHistoryDetailRaw(string partitionKey, string rowKey, string applicationName, DateTime timeStamp, string user, Guid folder, bool isAdd = false) 
            : base(partitionKey, rowKey, applicationName, timeStamp, user, folder, isAdd) { }

        [JsonConverter(typeof(RawJsonConverter))]
        public string NewJson { get; set; }
        [JsonConverter(typeof(RawJsonConverter))]
        public string OldJson { get; set; }
        [JsonConverter(typeof(RawJsonConverter))]
        public string Diff { get; set; }
    }

    public class ObjectHistoryDetailRead : ObjectHistoryDetailBase
    {
        public ObjectHistoryDetailRead(string partitionKey, string rowKey, string applicationName, DateTime timeStamp, string user, Guid folder, bool isAdd = false )
            : base(partitionKey, rowKey, applicationName, timeStamp, user, folder, isAdd) { }

        public object NewJson { get; set; }
        public object OldJson { get; set; }
        public object Diff { get; set; }

        public ObjectHistoryDetailRaw GetObjectHistoryDetailRaw()
        {
            try
            {
                if (this.IsAdd)
                {
                    return new ObjectHistoryDetailRaw(this.PartitionKey, this.RowKey, this.ApplicationName, this.TimeStamp, this.User, this.Folder, this.IsAdd)
                    {
                        NewJson = this.NewJson.ToString(),
                        OldJson = null,
                        Diff = null
                    };
                }
                else
                {
                    return new ObjectHistoryDetailRaw(this.PartitionKey, this.RowKey, this.ApplicationName, this.TimeStamp, this.User, this.Folder, this.IsAdd)
                    {
                        NewJson = this.NewJson.ToString(),
                        OldJson = this.OldJson.ToString(),
                        Diff = this.Diff.ToString()
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