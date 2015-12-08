using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    public class FlavorReference : IHaveExtraData, IServiceResource
    {
        /// <summary />
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary /> // In some cases, only the id is populated. Use GetFlavor if Name is null.
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary />
        [JsonExtensionData]
        IDictionary<string, JToken> IHaveExtraData.Data { get; set; } = new Dictionary<string, JToken>();

        object IServiceResource.Owner { get; set; }

        /// <inheritdoc cref="ComputeApiBuilder.GetFlavorAsync{T}" />
        /// <exception cref="InvalidOperationException">When the <see cref="FlavorReference"/> instance was not constructed by the <see cref="ComputeService"/>, as it is missing the appropriate internal state to execute service calls.</exception>
        public Task<Flavor> GetFlavorAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var owner = this.TryGetOwner<ComputeApiBuilder>();
            return owner.GetFlavorAsync<Flavor>(Id, cancellationToken);
        }
    }
}