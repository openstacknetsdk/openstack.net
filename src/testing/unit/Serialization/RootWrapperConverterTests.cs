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
        public void SerializeWhenNotRoot()
        {
            var json = JsonConvert.SerializeObject(new List<Thing>{ new Thing() });

            Assert.DoesNotContain("\"thing\"", json);
        }

        [Fact]
        public void SerializeWhenNested()
        {
            var json = JsonConvert.SerializeObject(new ThingCollection { new Thing() });

            Assert.DoesNotContain("\"thing\"", json);
        }

        [Fact]
        public void DeserializeWhenNotRoot()
        {
            var json = JArray.Parse("[{'id':'thing-id'}]").ToString();
            var things = JsonConvert.DeserializeObject<List<Thing>>(json);
            Assert.Equal(1, things.Count);
            Assert.Equal("thing-id", things[0].Id);
        }

        [Fact]
        public void DeserializeWhenNested()
        {
            var json = JObject.Parse("{'things':[{'id':'thing-id'}]}").ToString();
            var things = JsonConvert.DeserializeObject<ThingCollection>(json);
            Assert.NotNull(things);
            Assert.Equal(1, things.Count);
            Assert.Equal("thing-id", things[0].Id);
        }

        [Fact]
        public void ShouldIgnoreUnexpectedRootProperties()
        {
            var json = JObject.Parse("{'links': [{'name': 'next', 'link': 'http://nextlink'}], 'thing': {'id': 'thing-id'}}").ToString();
            var thing = JsonConvert.DeserializeObject<Thing>(json);
            Assert.NotNull(thing);
            Assert.Equal("thing-id", thing.Id);
        }
    }
}