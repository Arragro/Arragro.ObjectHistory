using Arragro.ObjectHistory.Core.Helpers;
using Newtonsoft.Json;
using System;

namespace Arragro.ObjectHistory.Core.Models
{
    public class ObjectHistoryDetailRaw : ObjectHistoryDetailBase
    {
        internal ObjectHistoryDetailRaw(ObjectHistorySettingsBase objectHistorySettingsBase, string partitionKey, string rowKey, string applicationName, DateTime timeStamp, string user, Guid folder, string securityValidationToken, Guid? subFolder, bool isAdd = false)
            : base(partitionKey, rowKey, applicationName, timeStamp, user, folder, isAdd, subFolder, securityValidationToken)
        {
            ObjectHistorySettingsBase = objectHistorySettingsBase;
        } 

        public ObjectHistoryDetailRaw(ObjectHistorySettingsBase objectHistorySettingsBase, string partitionKey, string applicationName, string user, Guid? folder = null, string securityValidationToken = null, bool isAdd = false)
            : base(partitionKey, string.Format("{0:D19}", DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks), applicationName, DateTime.UtcNow, user, folder.HasValue ? folder.Value : Guid.NewGuid(), isAdd, folder.HasValue ? Guid.NewGuid() : (Guid?)null, securityValidationToken)
        {
            ObjectHistorySettingsBase = objectHistorySettingsBase;
        }

        public ObjectHistorySettingsBase ObjectHistorySettingsBase { get; set; }
        [JsonConverter(typeof(RawJsonConverter))]
        public string NewJson { get; set; }
        [JsonConverter(typeof(RawJsonConverter))]
        public string OldJson { get; set; }
        [JsonConverter(typeof(RawJsonConverter))]
        public string Diff { get; set; }

        public T GetNew<T>()
        {
            return JsonConvert.DeserializeObject<T>(NewJson);
        }

        public T GetOld<T>()
        {
            return JsonConvert.DeserializeObject<T>(OldJson);
        }
    }
}
