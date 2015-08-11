using Newtonsoft.Json;
using Xunit;

namespace OpenStack.Serialization
{
    public class TolerantEnumConverterTests
    {
        [JsonConverter(typeof(TolerantEnumConverter))]
        enum ThingStatus
        {
            Active,
            Unknown
        }

        [JsonConverter(typeof(TolerantEnumConverter))]
        enum StuffStatus
        {
            Missing,
            Present
        }

        [Fact]
        public void WhenValueIsRecognized_MatchToValue()
        {
            var result = JsonConvert.DeserializeObject<ThingStatus>("\"Active\"");

            Assert.Equal(ThingStatus.Active, result);
        }

        [Fact]
        public void WhenValueIsUnrecognized_MatchToUnknownValue()
        {
            var result = JsonConvert.DeserializeObject<ThingStatus>("\"bad-enum-value\"");

            Assert.Equal(ThingStatus.Unknown, result);
        }

        [Fact]
        public void WhenValueIsUnrecognized_AndUnknownIsNotPresent_MatchToFirstValue()
        {
            var result = JsonConvert.DeserializeObject<StuffStatus>("\"bad-enum-value\"");

            Assert.Equal(StuffStatus.Missing, result);
        }

        [Fact]
        public void WhenValueIsUnrecognized_AndDestinationIsNullable_UseNull()
        {
            var result = JsonConvert.DeserializeObject<StuffStatus?>("\"bad-enum-value\"");

            Assert.Null(result);
        }
    }
}
