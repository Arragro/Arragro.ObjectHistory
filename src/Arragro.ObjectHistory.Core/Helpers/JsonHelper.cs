using Newtonsoft.Json;
using System;

namespace Arragro.ObjectHistory.Core.Helpers
{
    public class JsonHelper
    {
        private readonly Newtonsoft.Json.JsonSerializerSettings _jsonSettings;

        public JsonHelper()
        {
            _jsonSettings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.All
            };
        }

        public string GetJson<T>(T customObject, bool preserveReferencesHandling = false)
        {
            try
            {
                if (preserveReferencesHandling)
                {
                    return JsonConvert.SerializeObject(customObject, Formatting.Indented, _jsonSettings);
                }
                else
                {
                    return JsonConvert.SerializeObject(customObject, Formatting.Indented);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("There was an issue with serializing the object to json, please review the exception and retry. - {0}", ex.InnerException));
            }
        }

        public T GetObjectFromJson<T>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("There was an issue with deserializing the json into object {0} , please review the exception and retry. - {1}", typeof(T).FullName, ex.InnerException));
            }
            
        }

    }
}
