using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenStack.Compute.v2_1.Serialization;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    [JsonConverter(typeof(KeyPairConverter))]
    public class KeyPair : IHaveExtraData, IServiceResource
    {
        /// <summary />
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary />
        [JsonProperty("public_key")]
        public string PublicKey { get; set; }

        /// <summary />
        [JsonProperty("fingerprint")]
        public string Fingerprint { get; set; }

        /// <summary />
        [JsonExtensionData]
        IDictionary<string, JToken> IHaveExtraData.Data { get; set; } = new Dictionary<string, JToken>();

        object IServiceResource.Owner { get; set; }

        /// <inheritdoc cref="ComputeApiBuilder.DeleteKeyPairAsync" />
        public Task DeleteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var compute = this.GetOwnerOrThrow<ComputeApiBuilder>();
            return compute.DeleteKeyPairAsync(Name, cancellationToken);
        }
    }
}