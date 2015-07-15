using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace OpenStack.Serialization
{
    public class RootWrapperConverterTests
    {
        [JsonConverterWithConstructor(typeof(RootWrapperConverter), "thing")]
        class Thing
        {
            [JsonProperty("id")]
            public string Id { get; set; }
        }

        [JsonConverterWithConstructor(typeof(RootWrapperConverter), "things")]
        class ThingCollection : List<Thing>
        {
            public ThingCollection()
            {
            }

            public ThingCollection(IEnumerable<Thing> collection) : base(collection)
            {
            }
        }

        public RootWrapperConverterTests()
        {
            OpenStackNet.Configure();            
        }

        [Fact]
        public void Serialize()
        {
            var json = JsonConvert.SerializeObject(new Thing());
            
            var jsonObj = JObject.Parse(json);
            JProperty rootProperty = jsonObj.Properties().FirstOrDefault();
            Assert.NotNull(rootProperty);
            Assert.Equal("thing", rootProperty.Name);
        }

        [Fact]
        public void Deserialize()
        {
            var json = JsonConvert.SerializeObject(new Thing {Id = "thing-id"});
            var thing = JsonConvert.DeserializeObject<Thing>(json);
            Assert.Equal("thing-id", thing.Id);
        }

        [Fact]
        public void SerializeCollection()
        {
            var json = JsonConvert.SerializeObject(new ThingCollection(new[] { new Thing() }));

            Assert.Contains("\"things\"", json);
            Assert.DoesNotContain("\"thing\"", json);
        }

        [Fact]
        public void DeserializeCollection()
        {
            var json = JsonConvert.SerializeObject(new ThingCollection(new[] { new Thing{ Id = "thing-id"} }));
            var things = JsonConvert.DeserializeObject<ThingCollection>(json);
            Assert.Equal(1, things.Count);
            Assert.Equal("thing-id", things[0].Id);
        }
    }
}