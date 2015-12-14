using System;
using System.Collections.Generic;
using System.Extensions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    [JsonConverterWithConstructor(typeof(RootWrapperConverter), "server")]
    public class Server : ServerReference, IServiceResource
    {
        private string _adminPassword;

        /// <summary />
        [JsonProperty("addresses")]
        public IDictionary<string, IList<ServerAddress>> Addresses { get; set; }

        /// <summary />
        [JsonProperty("flavor")]
        public FlavorReference Flavor { get; set; }

        /// <summary />
        [JsonProperty("created")]
        public DateTimeOffset? Created { get; set; }

        /// <summary />
        [JsonProperty("image")]
        public ImageReference Image { get; set; }

        /// <summary /> // null if this isn't a newly created server. You only get this value once
        [JsonProperty("adminPass")]
        public string AdminPassword
        {
            get { return _adminPassword; }
            set
            {
                // This is only set once, then never again. Capture it for safekeeping
                _adminPassword = value ?? _adminPassword;
            }
        }

        /// <summary />
        [JsonProperty("key_name")]
        public string KeyPairName { get; set; }

        /// <summary />
        [JsonProperty("metadata")]
        public IDictionary<string, string> Metadata { get; set; }

        /// <summary />
        [JsonProperty("accessIPv4")]
        public string IPv4Address { get; set; }

        /// <summary />
        [JsonProperty("accessIPv6")]
        public string IPv6Address { get; set; }

        /// <summary />
        [JsonProperty("hostId")]
        public string HostId { get; set; }

        /// <summary />
        [JsonProperty("OS-DCF:diskConfig")]
        public DiskConfiguration DiskConfig { get; set; }

        /// <summary />
        [JsonProperty("OS-EXT-AZ:availability_zone")]
        public string AvailabilityZone { get; set; }

        /// <summary />
        [JsonProperty("OS-EXT-SRV-ATTR:host")]
        public string HostName { get; set; }

        /// <summary />
        [JsonProperty("OS-EXT-SRV-ATTR:hypervisor_hostname")]
        public string HypervisorHostName { get; set; }

        /// <summary />
        [JsonProperty("OS-EXT-SRV-ATTR:instance_name")]
        public string InstanceName { get; set; }

        /// <summary />
        [JsonProperty("OS-EXT-STS:power_state")]
        public string PowerState { get; set; }

        /// <summary />
        [JsonProperty("OS-EXT-STS:task_state")]
        public string TaskState { get; set; }

        /// <summary />
        [JsonProperty("OS-EXT-STS:vm_state")]
        public string VMState { get; set; }
        
        /// <summary />
        [JsonProperty("OS-SRV-USG:launched_at")]
        public DateTimeOffset? Launched { get; set; }

        /// <summary />
        [JsonProperty("OS-SRV-USG:terminated_at")]
        public DateTimeOffset? Terminated { get; set; }

        /// <summary />
        [JsonProperty("progress")]
        public int Progress { get; set; }

        /// <summary />
        [JsonProperty("os-extended-volumes:volumes_attached")]
        public IList<VolumeReference> AttachedVolumes { get; set; }

        /// <summary />
        [JsonProperty("security_groups")]
        public IList<SecurityGroupReference> SecurityGroups { get; set; }

        /// <summary />
        [JsonProperty("status")]
        public ServerStatus Status { get; set; }

        /// <summary />
        [JsonProperty("updated")]
        public DateTimeOffset? LastModified { get; set; }

        object IServiceResource.Owner { get; set; }
        
        /// <exception cref="InvalidOperationException">When the <see cref="Server"/> instance was not constructed by the <see cref="ComputeService"/>, as it is missing the appropriate internal state to execute service calls.</exception>
        public Task WaitUntilActiveAsync(TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return WaitForStatusAsync(ServerStatus.Active, refreshDelay, timeout, progress, cancellationToken);
        }

        /// <inheritdoc cref="ComputeApiBuilder.WaitUntilServerIsDeletedAsync" />
        /// <exception cref="InvalidOperationException">When the <see cref="Server"/> instance was not constructed by the <see cref="ComputeService"/>, as it is missing the appropriate internal state to execute service calls.</exception>
        public async Task WaitUntilDeletedAsync(TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var owner = this.TryGetOwner<ComputeApiBuilder>();
            await owner.WaitUntilServerIsDeletedAsync<Server, ServerStatus>(Id, refreshDelay, timeout, progress, cancellationToken).ConfigureAwait(false);
            Status = ServerStatus.Deleted;
        }

        /// <inheritdoc cref="ComputeApiBuilder.WaitForServerStatusAsync{TServer,TStatus}" />
        /// <exception cref="InvalidOperationException">When the <see cref="Server"/> instance was not constructed by the <see cref="ComputeService"/>, as it is missing the appropriate internal state to execute service calls.</exception>
        public async Task WaitForStatusAsync(ServerStatus status, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var owner = this.TryGetOwner<ComputeApiBuilder>();
            var result = await owner.WaitForServerStatusAsync<Server, ServerStatus>(Id, status, refreshDelay, timeout, progress, cancellationToken).ConfigureAwait(false);
            result.CopyProperties(this);
        }

        /// <inheritdoc cref="ComputeApiBuilder.UpdateServerAsync{T}" />
        /// <exception cref="InvalidOperationException">When the <see cref="Server"/> instance was not constructed by the <see cref="ComputeService"/>, as it is missing the appropriate internal state to execute service calls.</exception>
        public async Task UpdateAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var compute = this.TryGetOwner<ComputeApiBuilder>();
            var request = new ServerUpdateDefinition();
            this.CopyProperties(request);

            var result = await compute.UpdateServerAsync<Server>(Id, request, cancellationToken);
            result.CopyProperties(this);
        }
    }
}