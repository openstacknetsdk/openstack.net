using System.Linq;
using Newtonsoft.Json.Linq;

namespace net.openstack.Core.Domain.Mapping
{
    internal class NetworkResponseJsonMapper : IJsonObjectMapper<Network>
    {
        public Network Map(JObject @from)
        {
            if (from == null)
                return null;

            var network = from.Properties().First();
            return new Network
                {
                    Id = network.Name,
                    Addresses = network.Value.Select(o => o.ToObject<AddressDetails>()).ToArray()
                };
        }

        public JObject Map(Network to)
        {
            throw new System.NotImplementedException();
        }

        public Network Map(string rawJson)
        {
            if (string.IsNullOrWhiteSpace(rawJson))
                return null;

            return Map(JObject.Parse(rawJson));
        }
    }
}
