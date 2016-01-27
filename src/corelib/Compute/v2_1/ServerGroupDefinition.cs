using System.Collections.Generic;
using Newtonsoft.Json;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    [JsonConverterWithConstructor(typeof(RootWrapperConverter), "server_group")]
    public class ServerGroupDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerGroupDefinition"/> class.
        /// </summary>
        public ServerGroupDefinition(string name, params string[] policies)
        {
            Name = name;
            Policies = new List<string>(policies);
        }

        /// <summary>
        /// The server group name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// A list of one or more policy names to associate with the server group.
        /// </summary>
        [JsonProperty("policies")]
        public IList<string> Policies { get; set; }
    }
}