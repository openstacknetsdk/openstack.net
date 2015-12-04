using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    public class ServerReference : IHaveExtraData, IServiceResource
    {
        /// <summary />
        [JsonProperty("id")]
        public Identifier Id { get; set; }

        /// <summary />
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary />
        [JsonExtensionData]
        IDictionary<string, JToken> IHaveExtraData.Data { get; set; } = new Dictionary<string, JToken>();

        object IServiceResource.Owner { get; set; }
        
        /// <summary />
        public async Task<Server> GetServerAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var compute = this.GetOwnerOrThrow<ComputeApiBuilder>();
            return await compute.GetServerAsync<Server>(Id, cancellationToken).ConfigureAwait(false);
        }  
    }
}