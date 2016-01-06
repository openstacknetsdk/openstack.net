using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace OpenStack.Serialization
{
    /// <summary />
    public interface IHaveExtraData
    {
        /// <summary />
        IDictionary<string, JToken> Data { get; set; }
    }

    /// <summary />
    public static class IHaveExtraDataExtensions
    {
        /// <summary />
        public static T GetExtraData<T>(this IHaveExtraData dataContainer, string key)
            where T : class
        {
            JToken jsonValue;
            if (dataContainer.Data.TryGetValue(key, out jsonValue))
                return jsonValue.Value<T>();
            
            return null;
        }

        /// <summary />
        public static void SetExtraData(this IHaveExtraData dataContainer, string key, object value)
        {
            dataContainer.Data[key] = value != null ? JToken.FromObject(value) : JValue.CreateNull();
        }
    }
}