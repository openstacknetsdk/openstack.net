using System;
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
    public class ServerActionSummary : IHaveExtraData, IServiceResource
    {
        /// <summary />
        [JsonProperty("request_id")]
        public Identifier Id { get; set; }

        /// <summary />
        [JsonProperty("instance_uuid")]
        public Identifier ServerId { get; set; }

        /// <summary />
        [JsonProperty("action")]
        public string Name { get; set; }

        /// <summary />
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary />
        [JsonProperty("user_id")]
        public Identifier UserId { get; set; }

        /// <summary />
        [JsonProperty("start_time")]
        public DateTimeOffset Started { get; set; }

        /// <summary />
        [JsonExtensionData]
        IDictionary<string, JToken> IHaveExtraData.Data { get; set; } = new Dictionary<string, JToken>();

        object IServiceResource.Owner { get; set; }

        /// <summary />
        /// <exception cref="InvalidOperationException">When this instance was not constructed by the <see cref="ComputeService"/>, as it is missing the appropriate internal state to execute service calls.</exception>
        public Task<ServerAction> GetActionAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var compute = this.GetOwnerOrThrow<ComputeApi>();
            return compute.GetServerActionAsync<ServerAction>(ServerId, Id, cancellationToken);
        }
    }
}