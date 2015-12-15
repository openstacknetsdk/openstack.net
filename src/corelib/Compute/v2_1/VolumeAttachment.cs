using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    [JsonConverterWithConstructor(typeof(RootWrapperConverter), "volumeAttachment")]
    public class VolumeAttachment : VolumeReference
    {
        /// <summary />
        [JsonProperty("device")]
        public string DeviceName { get; set; }

        /// <summary />
        [JsonProperty("serverId")]
        public Identifier ServerId { get; set; }

        /// <summary />
        [JsonProperty("volumeId")]
        public Identifier VolumeId { get; set; }

        /// <summary />
        /// <exception cref="InvalidOperationException">When this instance was not constructed by the <see cref="ComputeService"/>, as it is missing the appropriate internal state to execute service calls.</exception>
        public override Task DetachAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var compute = this.GetOwnerOrThrow<ComputeApiBuilder>();
            return compute.DetachVolumeAsync(ServerId, VolumeId, cancellationToken);
        }
    }
}
