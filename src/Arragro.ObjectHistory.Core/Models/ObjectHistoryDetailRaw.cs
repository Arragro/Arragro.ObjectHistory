using Arragro.ObjectHistory.Core.Helpers;
using Newtonsoft.Json;
using System;

namespace Arragro.ObjectHistory.Core.Models
{
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
}
