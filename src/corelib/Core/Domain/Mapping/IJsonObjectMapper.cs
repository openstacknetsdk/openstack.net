using Newtonsoft.Json.Linq;

namespace net.openstack.Core.Domain.Mapping
{
    public interface IJsonObjectMapper<T>
    {
        T FromJson(string rawJson);

        JObject ToJson(T mapObj);
    }
}
