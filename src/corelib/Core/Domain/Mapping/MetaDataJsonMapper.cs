using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace net.openstack.Core.Domain.Mapping
{
    public class MetaDataJsonMapper : IJsonObjectMapper<Metadata>
    {
        public Metadata Map(string rawJson)
        {
            if (string.IsNullOrWhiteSpace(rawJson))
                return null;

            try
            {
                var json = JObject.Parse(rawJson);

                return Map(json);
            }
            catch (JsonReaderException)
            {
                return null;
            }
        }

        public Metadata Map(JObject json)
        {
            var metaDataItem = GetMetaDataItem(json);

            if (metaDataItem == null)
                return null;

            var metadata = new Metadata();
            foreach (var prop in metaDataItem.Children().OfType<JProperty>())
            {
                metadata.Add(prop.Name, prop.Value.Value<string>());
            }

            return metadata;
        }

        public JObject Map(Metadata mapObj)
        {
            throw new System.NotImplementedException();
        }

        private JObject GetMetaDataItem(JObject parent)
        {
            var item = parent["metadata"];

            if(item == null)
                item = parent.Descendants().OfType<JObject>().Select(o => o["metadata"]).FirstOrDefault();

            return item as JObject;
        }
    }
}
