using Arragro.ObjectHistory.Core.Helpers;
using Newtonsoft.Json;
using System;

namespace Arragro.ObjectHistory.Core.Models
{
    public class ObjectHistoryDetailRaw : ObjectHistoryDetailBase
    {
        internal ObjectHistoryDetailRaw(ObjectHistorySettings objectHistorySettings, string partitionKey, string rowKey, string applicationName, DateTime timeStamp, string user, Guid folder, string securityValidationToken, bool isAdd = false)
            : base(partitionKey, rowKey, applicationName, timeStamp, user, folder, isAdd)
        {
            ObjectHistorySettings = objectHistorySettings;
        }

        public ObjectHistoryDetailRaw(ObjectHistorySettings objectHistorySettings, string partitionKey, string applicationName, DateTime timeStamp, string user, bool isAdd = false)
            : base(partitionKey, string.Format("{0:D19}", DateTime.MaxValue.Ticks - timeStamp.Ticks), applicationName, timeStamp, user, Guid.NewGuid(), isAdd)
        {
            ObjectHistorySettings = objectHistorySettings;
        }

        public ObjectHistoryDetailRaw(ObjectHistorySettings objectHistorySettings, string partitionKey, string applicationName, string user, string securityValidationToken, bool isAdd = false)
            : base(partitionKey, string.Format("{0:D19}", DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks), applicationName, DateTime.UtcNow, user, Guid.NewGuid(), isAdd, securityValidationToken)
        {
            ObjectHistorySettings = objectHistorySettings;
        }

        public ObjectHistoryDetailRaw(ObjectHistorySettings objectHistorySettings, string partitionKey, string applicationName, string user, bool isAdd = false)
            : base(partitionKey, string.Format("{0:D19}", DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks), applicationName, DateTime.UtcNow, user, Guid.NewGuid(), isAdd)
        {
            ObjectHistorySettings = objectHistorySettings;
        }

        public ObjectHistorySettings ObjectHistorySettings { get; set; }
        [JsonConverter(typeof(RawJsonConverter))]
        public string NewJson { get; set; }
        [JsonConverter(typeof(RawJsonConverter))]
        public string OldJson { get; set; }
        [JsonConverter(typeof(RawJsonConverter))]
        public string Diff { get; set; }
    }
}
