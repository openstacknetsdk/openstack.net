using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace net.openstack.Core.Domain.Mapping
{
    public class MetaDataJsonMapper : IJsonObjectMapper<Metadata>
    {
        public Metadata FromJson(string rawJson)
        {
            if (string.IsNullOrWhiteSpace(rawJson))
                return null;

            try
            {
                var json = JObject.Parse(rawJson);

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
            catch (JsonReaderException)
            {
                return null;
            }
        }

        public JObject ToJson(Metadata mapObj)
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
