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
    public class VolumeReference : IHaveExtraData, IServiceResource
    {
        /// <summary />
        [JsonIgnore]
        protected internal ServerReference Server { get; set; }

        /// <summary />
        public Identifier Id { get; set; }

        object IServiceResource.Owner { get; set; }

        /// <summary />
        [JsonExtensionData]
        IDictionary<string, JToken> IHaveExtraData.Data { get; set; } = new Dictionary<string, JToken>();

        /// <summary />
        /// <exception cref="InvalidOperationException">When this instance was not constructed by the <see cref="ComputeService"/>, as it is missing the appropriate internal state to execute service calls.</exception>
        public virtual Task DetachAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if(Server == null)
                throw new InvalidOperationException("Detach can only be used on instances which were constructed by the ComputeService. Use ComputeService.DetachVolume instead.");
                
            var compute = this.GetOwnerOrThrow<ComputeApiBuilder>();
            return compute.DetachVolumeAsync(Server.Id, Id, cancellationToken);
        }
    }
}