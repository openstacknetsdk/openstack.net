using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    public class VolumeReference : IHaveExtraData, IChildResource
    {
        /// <summary />
        [JsonIgnore]
        protected internal Server Server { get; set; }

        /// <summary />
        public Identifier Id { get; set; }

        object IServiceResource.Owner { get; set; }

        void IChildResource.SetParent(string parentId)
        {
            Server = new Server { Id = parentId };
        }

        /// <summary />
        [JsonExtensionData]
        IDictionary<string, JToken> IHaveExtraData.Data { get; set; } = new Dictionary<string, JToken>();

        /// <summary />
        protected void AssertServerIsSet([CallerMemberName]string callerName = "")
        {
            if (Server != null)
                return;

            throw new InvalidOperationException(string.Format($"{callerName} can only be used on instances which were constructed by the ComputeServer. Use ComputeService.{callerName} instead."));
        }

        /// <summary />
        /// <exception cref="InvalidOperationException">When this instance was not constructed by the <see cref="ComputeService"/>, as it is missing the appropriate internal state to execute service calls.</exception>
        public async Task<VolumeAttachment> GetServerVolumeAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            AssertServerIsSet();

            var compute = this.GetOwnerOrThrow<ComputeApiBuilder>();
            var result = await compute.GetServerVolumeAsync<VolumeAttachment>(Server.Id, Id, cancellationToken);
            result.Server = Server;
            return result;
        }

        /// <summary />
        /// <exception cref="InvalidOperationException">When this instance was not constructed by the <see cref="ComputeService"/>, as it is missing the appropriate internal state to execute service calls.</exception>
        public async Task DetachAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            AssertServerIsSet(); 

            var compute = this.GetOwnerOrThrow<ComputeApiBuilder>();
            await compute.DetachVolumeAsync(Server.Id, Id, cancellationToken);

            var attachedVolume = Server.AttachedVolumes.FirstOrDefault(v => v.Id == Id);
            if(attachedVolume != null)
                Server.AttachedVolumes.Remove(attachedVolume);
        }
    }
}