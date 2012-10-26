using System;
using Newtonsoft.Json.Linq;
using net.openstack.Core.Domain.Mapping;

namespace net.openstack.Providers.Rackspace.Objects.Mapping
{
    public class CreateServerRequestJsonMapper : IJsonObjectMapper<CreateServerRequest>
    {
        public CreateServerRequest FromJson(string rawJson)
        {
            throw new NotImplementedException();
        }

        public JObject ToJson(CreateServerRequest mapObj)
        {
            return new JObject { {"server", new JObject
                                             {
                                                 {"name", mapObj.Name.ToLower()}, 
                                                 {"imageRef", mapObj.ImageName}, 
                                                 {"flavorRef", mapObj.Flavor}, 
                                                 {"OS-DCF:diskConfig", mapObj.DiskConfig},
                                                 {"metadata", new JObject { { "My Server Name", mapObj.FriendlyName } } }
                                             }} };
        }
    }
}
