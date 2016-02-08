using System;
using System.Collections.Generic;
using System.Extensions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenStack.BlockStorage.v2;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    [JsonConverterWithConstructor(typeof(RootWrapperConverter), "snapshot")]
    public class VolumeSnapshot : IServiceResource, IHaveExtraData
    {
        /// <summary />
        [JsonProperty("id")]
        public Identifier Id { get; set; }

        /// <summary />
        [JsonProperty("volumeId")]
        public Identifier VolumeId { get; set; }

        /// <summary />
        [JsonProperty("displayName")]
        public string Name { get; set; }

        /// <summary />
        [JsonProperty("displayDescription")]
        public string Description { get; set; }

        /// <summary />
        [JsonProperty("size")]
        public int Size { get; set; }

        /// <summary />
        [JsonProperty("status")]
        public SnapshotStatus Status { get; set; }

        /// <summary />
        [JsonProperty("createdAt")]
        public DateTime Created { get; set; }

        /// <summary />
        [JsonExtensionData]
        IDictionary<string, JToken> IHaveExtraData.Data { get; set; } = new Dictionary<string, JToken>();

        object IServiceResource.Owner { get; set; }

        /// <exception cref="InvalidOperationException">When the instance was not constructed by the <see cref="ComputeService"/>, as it is missing the appropriate internal state to execute service calls.</exception>
        public Task WaitUntilAvailableAsync(TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return WaitForStatusAsync(SnapshotStatus.Available, refreshDelay, timeout, progress, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.WaitUntilVolumeIsDeletedAsync{TSnapshot,TStatus}" />
        /// <exception cref="InvalidOperationException">When the instance was not constructed by the <see cref="ComputeService"/>, as it is missing the appropriate internal state to execute service calls.</exception>
        public Task WaitUntilDeletedAsync(TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var owner = this.GetOwnerOrThrow<ComputeApiBuilder>();
            return owner.WaitUntilVolumeSnapshotIsDeletedAsync<VolumeSnapshot, SnapshotStatus>(Id, refreshDelay, timeout, progress, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.WaitForVolumeStatusAsync{TSnapshot,TStatus}(string,IEnumerable{TStatus},TimeSpan?,TimeSpan?,IProgress{bool},CancellationToken)" />
        /// <exception cref="InvalidOperationException">When the instance was not constructed by the <see cref="ComputeService"/>, as it is missing the appropriate internal state to execute service calls.</exception>
        public async Task WaitForStatusAsync(IEnumerable<SnapshotStatus> status, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var owner = this.GetOwnerOrThrow<ComputeApiBuilder>();
            var result = await owner.WaitForVolumeSnapshotStatusAsync<VolumeSnapshot, SnapshotStatus>(Id, status, refreshDelay, timeout, progress, cancellationToken).ConfigureAwait(false);
            result.CopyProperties(this);
        }

        /// <inheritdoc cref="ComputeApiBuilder.WaitForVolumeStatusAsync{TServer,TStatus}(string,TStatus,TimeSpan?,TimeSpan?,IProgress{bool},CancellationToken)" />
        /// <exception cref="InvalidOperationException">When the instance was not constructed by the <see cref="ComputeService"/>, as it is missing the appropriate internal state to execute service calls.</exception>
        public async Task WaitForStatusAsync(SnapshotStatus status, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var owner = this.GetOwnerOrThrow<ComputeApiBuilder>();
            var result = await owner.WaitForVolumeSnapshotStatusAsync<VolumeSnapshot, SnapshotStatus>(Id, status, refreshDelay, timeout, progress, cancellationToken).ConfigureAwait(false);
            result.CopyProperties(this);
        }

        /// <inheritdoc cref="ComputeApiBuilder.DeleteVolumeSnapshotAsync" />
        public Task DeleteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var owner = this.GetOwnerOrThrow<ComputeApiBuilder>();
            return owner.DeleteVolumeSnapshotAsync(Id, cancellationToken);
        }
    }
}