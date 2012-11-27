using Newtonsoft.Json.Linq;

namespace net.openstack.Core.Domain.Mapping
{
    public interface IJsonObjectMapper<T> : IObjectMapper<JObject, T>
    {
        T Map(string rawJson);
    }
}
