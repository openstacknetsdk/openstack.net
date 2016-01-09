using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenStack.Networking.v2.Serialization;
using OpenStack.Serialization;
using Xunit;

namespace OpenStack.Networking.v2
{
    public class DHCPOptionConverterTests
    {
        [Fact]
        public void Serialize()
        {
            var options = new Dictionary<string, string>
            {
                {"a", "stuff"},
                {"b", "things"}
            };

            string result = OpenStackNet.Configuration.FlurlHttpSettings.JsonSerializer.Serialize(options, Formatting.None, new DHCPOptionsConverter());

            string expectedJson = JArray.Parse("[{'opt_name':'a','opt_value':'stuff'},{'opt_name':'b','opt_value':'things'}]").ToString(Formatting.None);
            Assert.Equal(expectedJson, result);
        }

        [Fact]
        public void Deserialize()
        {
            string json = JArray.Parse("[{'opt_name':'a','opt_value':'stuff'},{'opt_name':'b','opt_value':'things'}]").ToString(Formatting.None);

            var result = OpenStackNet.Configuration.FlurlHttpSettings.JsonSerializer.Deserialize<Dictionary<string, string>>(json, new DHCPOptionsConverter());

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            
            Assert.Contains("a", result.Keys);
            Assert.Contains("b", result.Keys);
            Assert.Equal("stuff", result["a"]);
            Assert.Equal("things", result["b"]);
        }

        [Fact]
        public void OpenStackNet_UsesDHCPOptionConverter()
        {
            OpenStackNet.Configure();

            var port = new Port
            {
                DHCPOptions = new Dictionary<string, string>
                {
                    {"a", "stuff"}
                }
            };

            var json = OpenStackNet.Configuration.FlurlHttpSettings.JsonSerializer.Serialize(port);
            var result = OpenStackNet.Configuration.FlurlHttpSettings.JsonSerializer.Deserialize<Port>(json);

            Assert.NotNull(result.DHCPOptions);
            Assert.Equal(1, result.DHCPOptions.Count);
        }
    }
}
