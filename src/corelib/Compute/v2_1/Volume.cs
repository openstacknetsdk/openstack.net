using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    public class Volume : VolumeDefinition, IServiceResource
    {
        /// <summary>
        /// The volume identifier.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("attachments")]
        public IList<ServerVolume> Attachments { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("created_at")]
        public DateTime Created { get; set; }

        object IServiceResource.Owner { get; set; }

        // TOODO: Once we move over the storage provider, let's add WaitUntilActive, etc here as well

        /// <inheritdoc cref="ComputeApiBuilder.DeleteVolumeAsync" />
        public Task DeleteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var owner = this.GetOwnerOrThrow<ComputeApiBuilder>();
            return owner.DeleteVolumeAsync(Id, cancellationToken);
        }
    }
}