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
    public class ServerVolumeReference : IHaveExtraData, IChildResource
    {
        /// <summary />
        [JsonIgnore]
        protected ServerReference ServerRef { get; set; }

        /// <summary />
        public Identifier Id { get; set; }

        object IServiceResource.Owner { get; set; }

        /// <summary />
        protected internal void SetParent(ServerReference parent)
        {
            ServerRef = parent;
        }

        void IChildResource.SetParent(object parent)
        {
            SetParent((Server)parent);
        }

        void IChildResource.SetParent(string parentId)
        {
            SetParent(new ServerReference {Id = parentId});
        }

        /// <summary />
        [JsonExtensionData]
        IDictionary<string, JToken> IHaveExtraData.Data { get; set; } = new Dictionary<string, JToken>();

        /// <summary />
        protected void AssertServerIsSet([CallerMemberName]string callerName = "")
        {
            if (ServerRef != null)
                return;

            throw new InvalidOperationException(string.Format($"{callerName} can only be used on instances which were constructed by the ComputeServer. Use ComputeService.{callerName} instead."));
        }

        /// <summary />
        /// <exception cref="InvalidOperationException">When this instance was not constructed by the <see cref="ComputeService"/>, as it is missing the appropriate internal state to execute service calls.</exception>
        public async Task<ServerVolume> GetServerVolumeAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            AssertServerIsSet();

            var compute = this.GetOwnerOrThrow<ComputeApiBuilder>();
            var result = await compute.GetServerVolumeAsync<ServerVolume>(ServerRef.Id, Id, cancellationToken);
            result.ServerRef = ServerRef;
            return result;
        }

        /// <summary />
        /// <exception cref="InvalidOperationException">When this instance was not constructed by the <see cref="ComputeService"/>, as it is missing the appropriate internal state to execute service calls.</exception>
        public async Task DetachAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            AssertServerIsSet(); 

            var compute = this.GetOwnerOrThrow<ComputeApiBuilder>();
            await compute.DetachVolumeAsync(ServerRef.Id, Id, cancellationToken);

            var server = ServerRef as Server;
            if (server != null)
            {
                var attachedVolume = server.AttachedVolumes.FirstOrDefault(v => v.Id == Id);
                if (attachedVolume != null)
                    server.AttachedVolumes.Remove(attachedVolume);
            }
        }
    }
}