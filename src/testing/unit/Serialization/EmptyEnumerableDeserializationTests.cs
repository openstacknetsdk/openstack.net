using System.Collections.Generic;
using Newtonsoft.Json;
using Xunit;

namespace OpenStack.Serialization
{
    public class EmptyEnumerableDeserializationTests
    {
        private class ExampleThing
        {
            [JsonProperty("messages")]
            public IEnumerable<string> Messages { get; set; } 
        }

        [Fact]
        public void WhenDeserializingNullCollection_ItShouldUseAnEmptyCollection()
        {
            var settings = new JsonSerializerSettings {ContractResolver = new EmptyEnumerableResolver()};
            string json = JsonConvert.SerializeObject(new ExampleThing{Messages = null});
            var result = JsonConvert.DeserializeObject<ExampleThing>(json, settings);
            Assert.NotNull(result.Messages);
            Assert.Empty(result.Messages);
        }
    }
}
