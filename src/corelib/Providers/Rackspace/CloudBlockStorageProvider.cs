using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using JSIStudios.SimpleRESTServices.Client;
using JSIStudios.SimpleRESTServices.Client.Json;
using net.openstack.Core.Domain;
using net.openstack.Core.Exceptions.Response;
using net.openstack.Core.Providers;
using net.openstack.Core.Validators;
using net.openstack.Providers.Rackspace.Objects.Request;
using net.openstack.Providers.Rackspace.Objects.Response;
using net.openstack.Providers.Rackspace.Validators;
using CreateCloudBlockStorageVolumeDetails = net.openstack.Providers.Rackspace.Objects.Request.CreateCloudBlockStorageVolumeDetails;

namespace net.openstack.Providers.Rackspace
{
    /// <summary>
    /// <para>The Cloud Block Storage Provider enables simple access to the Rackspace Cloud Block Storage Volumes as well as Cloud Block Storage Volume Snapshot services.
    /// Rackspace Cloud Block Storage is a block level storage solution that allows customers to mount drives or volumes to their Rackspace Next Generation Cloud Servers.
    /// The two primary use cases are (1) to allow customers to scale their storage independently from their compute resources,
    /// and (2) to allow customers to utilize high performance storage to serve database or I/O-intensive applications.</para>
    /// <para />
    /// <para>Highlights of Rackspace Cloud Block Storage include:</para>
    /// <para>- Mount a drive to a Cloud Server to scale storage without paying for more compute capability.</para>
    /// <para>- A high performance option for databases and high performance applications, leveraging solid state drives for speed.</para>
    /// <para>- A standard speed option for customers who just need additional storage on their Cloud Server.</para>
    /// <para />
    /// <para>Notes:</para>
    /// <para>- Cloud Block Storage is an add-on feature to Next Generation Cloud Servers.  Customers may not attach Cloud Block Storage volumes to other instances, like first generation Cloud Servers.</para>
    /// <para>- Cloud Block Storage is multi-tenant rather than dedicated.</para>
    /// <para>- When volumes are destroyed, Rackspace keeps that disk space unavailable until zeros have been written to the space to ensure that data is not accessible by any other customers.</para>
    /// <para>- Cloud Block Storage allows you to create snapshots that you can save, list, and restore.</para>
    /// <para />
    /// <para>Documentation URL: http://docs.rackspace.com/cbs/api/v1.0/cbs-devguide/content/overview.html</para>
    /// </summary>
    /// <see cref="IBlockStorageProvider"/>
    /// <inheritdoc />
    public class CloudBlockStorageProvider : ProviderBase<IBlockStorageProvider>, IBlockStorageProvider
    {

        private readonly HttpStatusCode[] _validResponseCode = new[] { HttpStatusCode.OK, HttpStatusCode.Created, HttpStatusCode.Accepted };
        private readonly IBlockStorageValidator _cloudBlockStorageValidator;

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudBlockStorageProvider"/> class.
        /// </summary>
        public CloudBlockStorageProvider()
            : this(null, null, null) { }

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudBlockStorageProvider"/> class.
        /// </summary>
        /// <param name="identity">An instance of a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object.<remarks>If not provided, the user will be required to pass a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object to each method individually.</remarks></param>
        public CloudBlockStorageProvider(CloudIdentity identity)
            : this(identity, null, null) { }

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudBlockStorageProvider"/> class.
        /// </summary>
        /// <param name="restService">An instance of an <see cref="IRestService"/> to override the default <see cref="JsonRestServices"/></param>
        public CloudBlockStorageProvider(IRestService restService)
            : this(null, restService) { }
    
        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudBlockStorageProvider"/> class.
        /// </summary>
        /// <param name="identityProvider">An instance of an <see cref="IIdentityProvider"/> to override the default <see cref="CloudIdentity"/></param>
        public CloudBlockStorageProvider(IIdentityProvider identityProvider)
            : this(null, identityProvider, null) { }

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudBlockStorageProvider"/> class.
        /// </summary>
        /// /<param name="identity">An instance of a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object. <remarks>If not provided, the user will be required to pass a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object to each method individually.</remarks></param>
        /// <param name="identityProvider">An instance of an <see cref="IIdentityProvider"/> to override the default <see cref="CloudIdentity"/></param>
        public CloudBlockStorageProvider(CloudIdentity identity, IIdentityProvider identityProvider)
            : this(identity, identityProvider, null) { }

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudBlockStorageProvider"/> class.
        /// </summary>
        /// <param name="identity">An instance of a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object. <remarks>If not provided, the user will be required to pass a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object to each method individually.</remarks></param>
        /// <param name="restService">An instance of an <see cref="IRestService"/> to override the default <see cref="JsonRestServices"/></param>
        public CloudBlockStorageProvider(CloudIdentity identity, IRestService restService)
            : this(identity, null, restService) { }

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudBlockStorageProvider"/> class.
        /// </summary>
        /// <param name="identity">An instance of a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object. <remarks>If not provided, the user will be required to pass a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object to each method individually.</remarks></param>
        /// <param name="identityProvider">An instance of an <see cref="IIdentityProvider"/> to override the default <see cref="CloudIdentity"/></param>
        /// <param name="restService">An instance of an <see cref="IRestService"/> to override the default <see cref="JsonRestServices"/></param>
        public CloudBlockStorageProvider(CloudIdentity identity, IIdentityProvider identityProvider, IRestService restService)
            : this(identity, identityProvider, restService, CloudBlockStorageValidator.Default) { }


        internal CloudBlockStorageProvider(CloudIdentity identity, IIdentityProvider identityProvider, IRestService restService, IBlockStorageValidator cloudBlockStorageValidator)
            : base(identity, identityProvider, restService)
        {
            _cloudBlockStorageValidator = cloudBlockStorageValidator;
        }

        #region Volumes

        /// <inheritdoc />
        public bool CreateVolume(int size, string displayDescription = null, string displayName = null, string snapshotId = null, string volumeType = null, string region = null, CloudIdentity identity = null)
        {
            _cloudBlockStorageValidator.ValidateVolumeSize(size);

            var urlPath = new Uri(string.Format("{0}/volumes", GetServiceEndpoint(identity, region)));
            var requestBody = new CreateCloudBlockStorageVolumeRequest { CreateCloudBlockStorageVolumeDetails = new CreateCloudBlockStorageVolumeDetails { Size = size, DisplayDescription = displayDescription, DisplayName = displayName, SnapshotId = snapshotId, VolumeType = volumeType } };
            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.POST, requestBody);

            return response != null && _validResponseCode.Contains(response.StatusCode);
        }

        /// <inheritdoc />
        public IEnumerable<Volume> ListVolumes(string region = null, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}/volumes", GetServiceEndpoint(identity, region)));
            var response = ExecuteRESTRequest<ListVolumeResponse>(identity, urlPath, HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Volumes;
        }

        /// <inheritdoc />
        public Volume ShowVolume(string volumeId, string region = null, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}/volumes/{1}", GetServiceEndpoint(identity, region), volumeId));
            var response = ExecuteRESTRequest<GetCloudBlockStorageVolumeResponse>(identity, urlPath, HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Volume;
        }

        /// <inheritdoc />
        public bool DeleteVolume(string volumeId, string region = null, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}/volumes/{1}", GetServiceEndpoint(identity, region), volumeId));
            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.DELETE);

            return response != null && _validResponseCode.Contains(response.StatusCode);
        }

        /// <inheritdoc />
        public IEnumerable<VolumeType> ListVolumeTypes(string region = null, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}/types", GetServiceEndpoint(identity, region)));
            var response = ExecuteRESTRequest<ListVolumeTypeResponse>(identity, urlPath, HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return response.Data.VolumeTypes;
        }

        /// <inheritdoc />
        public VolumeType DescribeVolumeType(int volumeTypeId, string region = null, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}/types/{1}", GetServiceEndpoint(identity, region), volumeTypeId));
            var response = ExecuteRESTRequest<GetCloudBlockStorageVolumeTypeResponse>(identity, urlPath, HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return response.Data.VolumeType;
        }

        /// <inheritdoc />
        public Volume WaitForVolumeAvailable(string volumeId, int refreshCount = 600, TimeSpan? refreshDelay = null, string region = null, CloudIdentity identity = null)
        {
            return WaitForVolumeState(volumeId, VolumeState.AVAILABLE, new[] { VolumeState.ERROR, VolumeState.ERROR_DELETING }, refreshCount, refreshDelay ?? TimeSpan.FromMilliseconds(2400), region, identity);
        }

        /// <inheritdoc />
        public bool WaitForVolumeDeleted(string volumeId, int refreshCount = 360, TimeSpan? refreshDelay = null, string region = null, CloudIdentity identity = null)
        {
            return WaitForItemToBeDeleted(ShowVolume, volumeId, refreshCount, refreshDelay ?? TimeSpan.FromSeconds(10), region, identity);
        }

        /// <inheritdoc />
        public Volume WaitForVolumeState(string volumeId, string expectedState, string[] errorStates, int refreshCount = 600, TimeSpan? refreshDelay = null, string region = null, CloudIdentity identity = null)
        {
            var volumeInfo = ShowVolume(volumeId, region, identity);

            var count = 0;
            while (!volumeInfo.Status.Equals(expectedState, StringComparison.OrdinalIgnoreCase) && !errorStates.Contains(volumeInfo.Status) && count < refreshCount)
            {
                Thread.Sleep(refreshDelay ?? TimeSpan.FromMilliseconds(2400));
                volumeInfo = ShowVolume(volumeId, region, identity);
                count++;
            }

            if (errorStates.Contains(volumeInfo.Status))
                throw new VolumeEnteredErrorStateException(volumeInfo.Status);

            return volumeInfo;
        }

        /// <inheritdoc />
        public class VolumeEnteredErrorStateException : Exception
        {
            public string Status { get; private set; }

            public VolumeEnteredErrorStateException(string status)
                : base(string.Format("The volume entered an error state: '{0}'", status))
            {
                Status = status;
            }
        }

        #endregion

        #region Snapshots

        /// <inheritdoc />
        public bool CreateSnapshot(string volumeId, bool force = false, string displayName = "None", string displayDescription = "None", string region = null, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}/snapshots", GetServiceEndpoint(identity, region)));
            var requestBody = new CreateCloudBlockStorageSnapshotRequest { CreateCloudBlockStorageSnapshotDetails = new CreateCloudBlockStorageSnapshotDetails { VolumeId = volumeId, Force = force, DisplayName = displayName, DisplayDescription = displayDescription } };
            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.POST, requestBody);

            return response != null && _validResponseCode.Contains(response.StatusCode);
        }

        /// <inheritdoc />
        public IEnumerable<Snapshot> ListSnapshots(string region = null, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}/snapshots", GetServiceEndpoint(identity, region)));
            var response = ExecuteRESTRequest<ListSnapshotResponse>(identity, urlPath, HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Snapshots;
        }

        /// <inheritdoc />
        public Snapshot ShowSnapshot(string snapshotId, string region = null, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}/snapshots/{1}", GetServiceEndpoint(identity, region), snapshotId));
            var response = ExecuteRESTRequest<GetCloudBlockStorageSnapshotResponse>(identity, urlPath, HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Snapshot;
        }

        /// <inheritdoc />
        public bool DeleteSnapshot(string snapshotId, string region = null, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}/snapshots/{1}", GetServiceEndpoint(identity, region), snapshotId));
            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.DELETE);

            return response != null && _validResponseCode.Contains(response.StatusCode);
        }

        /// <inheritdoc />
        public Snapshot WaitForSnapshotAvailable(string snapshotId, int refreshCount = 360, TimeSpan? refreshDelay = null, string region = null, CloudIdentity identity = null)
        {
            return WaitForSnapshotState(snapshotId, SnapshotState.AVAILABLE, new[] { SnapshotState.ERROR, SnapshotState.ERROR_DELETING }, refreshCount, refreshDelay ?? TimeSpan.FromSeconds(10), region, identity);
        }

        /// <inheritdoc />
        public bool WaitForSnapshotDeleted(string snapshotId, int refreshCount = 180, TimeSpan? refreshDelay = null, string region = null, CloudIdentity identity = null)
        {
            return WaitForItemToBeDeleted(ShowSnapshot, snapshotId, refreshCount, refreshDelay ?? TimeSpan.FromSeconds(10), region, identity);
        }

        /// <inheritdoc />
        public Snapshot WaitForSnapshotState(string snapshotId, string expectedState, string[] errorStates, int refreshCount = 60, TimeSpan? refreshDelay = null, string region = null, CloudIdentity identity = null)
        {
            var snapshotInfo = ShowSnapshot(snapshotId, region, identity);

            var count = 0;
            while (!snapshotInfo.Status.Equals(expectedState, StringComparison.OrdinalIgnoreCase) && !errorStates.Contains(snapshotInfo.Status) && count < refreshCount)
            {
                Thread.Sleep(refreshDelay ?? TimeSpan.FromSeconds(10));
                snapshotInfo = ShowSnapshot(snapshotId, region, identity);
                count++;
            }

            if (errorStates.Contains(snapshotInfo.Status))
                throw new SnapshotEnteredErrorStateException(snapshotInfo.Status);

            return snapshotInfo;
        }

        /// <inheritdoc />
        public class SnapshotEnteredErrorStateException : Exception
        {
            public string Status { get; private set; }

            public SnapshotEnteredErrorStateException(string status)
                : base(string.Format("The snapshot entered an error state: '{0}'", status))
            {
                Status = status;
            }
        }

        #endregion

        #region Private methods

        protected string GetServiceEndpoint(CloudIdentity identity = null, string region = null)
        {
            return base.GetPublicServiceEndpoint(identity, "volume", region);
        }

        private bool WaitForItemToBeDeleted<T>(Func<string, string, CloudIdentity, T> retrieveItemMethod, string id, int refreshCount = 360, TimeSpan? refreshDelay = null, string region = null, CloudIdentity identity = null)
        {
            try
            {
                retrieveItemMethod(id, region, identity);

                var count = 0;
                while (count < refreshCount)
                {
                    Thread.Sleep(refreshDelay ?? TimeSpan.FromSeconds(10));
                    retrieveItemMethod(id, region, identity);
                    count++;
                }
            }
            catch (ItemNotFoundException)
            {
                return true;
            }

            return false;
        }

        protected override IBlockStorageProvider BuildProvider(CloudIdentity identity)
        {
            return new CloudBlockStorageProvider(identity, IdentityProvider, RestService, _cloudBlockStorageValidator);
        }
        #endregion

        
    }
}
