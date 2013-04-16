using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JSIStudios.SimpleRESTServices.Client;
using JSIStudios.SimpleRESTServices.Client.Json;
using net.openstack.Core;
using net.openstack.Core.Domain;
using net.openstack.Core.Exceptions.Response;
using net.openstack.Providers.Rackspace.Objects.Request;
using net.openstack.Providers.Rackspace.Objects.Response;
using net.openstack.Providers.Rackspace.Validators;
using CreateCloudBlockStorageVolumeDetails = net.openstack.Providers.Rackspace.Objects.Request.CreateCloudBlockStorageVolumeDetails;

namespace net.openstack.Providers.Rackspace
{
    /// <summary>
    /// The Cloud Block Storage Provider contains the methods required to interact with Cloud Block Storage Volumes as well as Cloud Block Storage Volume Snapshots.
    /// Rackspace Cloud Block Storage is a block level storage solution that allows customers to mount drives or volumes to their Rackspace Next Generation Cloud Servers.
    /// The two primary use cases are (1) to allow customers to scale their storage independently from their compute resources,
    /// and (2) to allow customers to utilize high performance storage to serve database or I/O-intensive applications.
    /// 
    /// Highlights of Rackspace Cloud Block Storage include:
    /// - Mount a drive to a Cloud Server to scale storage without paying for more compute capability.
    /// - A high performance option for databases and high performance applications, leveraging solid state drives for speed.
    /// - A standard speed option for customers who just need additional storage on their Cloud Server.
    /// 
    /// Notes:
    /// - Cloud Block Storage is an add-on feature to Next Generation Cloud Servers.  Customers may not attach Cloud Block Storage volumes to other instances, like first generation Cloud Servers.
    /// - Cloud Block Storage is multi-tenant rather than dedicated.
    /// - When volumes are destroyed, Rackspace keeps that disk space unavailable until zeros have been written to the space to ensure that data is not accessible by any other customers.
    /// - Cloud Block Storage allows you to create snapshots that you can save, list, and restore.
    /// 
    /// Documentation URL: http://docs.rackspace.com/cbs/api/v1.0/cbs-devguide/content/overview.html
    /// </summary>
    public class CloudBlockStorageProvider : ProviderBase, ICloudBlockStorageProvider
    {

        private readonly int[] _validResponseCode = new[] { 200, 201, 202 };
        private readonly ICloudBlockStorageValidator _cloudBlockStorageValidator;

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudBlockStorageProvider"/> class.
        /// </summary>
        public CloudBlockStorageProvider()
            : this(null) { }

        /// <summary>
        /// Creates a new instance of the Rackspace <see cref="net.openstack.Providers.Rackspace.CloudBlockStorageProvider"/> class.
        /// </summary>
        /// <param name="identity">An instance of a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object.<remarks>If not provided, the user will be required to pass a <see cref="net.openstack.Core.Domain.CloudIdentity"/> object to each method individually.</remarks></param>
        public CloudBlockStorageProvider(CloudIdentity identity)
            : this(identity, new CloudIdentityProvider(), new JsonRestServices(), new CloudBlockStorageValidator()) { }

        internal CloudBlockStorageProvider(ICloudIdentityProvider identityProvider, IRestService restService, ICloudBlockStorageValidator cloudBlockStorageValidator)
            : this(null, identityProvider, restService, cloudBlockStorageValidator) { }

        internal CloudBlockStorageProvider(CloudIdentity identity, ICloudIdentityProvider identityProvider, IRestService restService, ICloudBlockStorageValidator cloudBlockStorageValidator)
            : base(identity, identityProvider, restService)
        {
            _cloudBlockStorageValidator = cloudBlockStorageValidator;
        }

        #region Volumes


        public bool CreateVolume(int size, string displayDescription = null, string displayName = null, string snapshotId = null, string volumeType = null, string region = null, CloudIdentity identity = null)
        {
            _cloudBlockStorageValidator.ValidateVolumeSize(size);

            var urlPath = new Uri(string.Format("{0}/volumes", GetServiceEndpoint(identity, region)));
            var requestBody = new CreateCloudBlockStorageVolumeRequest { CreateCloudBlockStorageVolumeDetails = new CreateCloudBlockStorageVolumeDetails { Size = size, DisplayDescription = displayDescription, DisplayName = displayName, SnapshotId = snapshotId, VolumeType = volumeType } };
            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.POST, requestBody);

            return response != null && _validResponseCode.Contains(response.StatusCode);
        }

        public IEnumerable<Volume> ListVolumes(string region = null, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}/volumes", GetServiceEndpoint(identity, region)));
            var response = ExecuteRESTRequest<ListVolumeResponse>(identity, urlPath, HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Volumes;
        }

        public Volume ShowVolume(string volumeId, string region = null, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}/volumes/{1}", GetServiceEndpoint(identity, region), volumeId));
            var response = ExecuteRESTRequest<GetCloudBlockStorageVolumeResponse>(identity, urlPath, HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Volume;
        }

        public bool DeleteVolume(string volumeId, string region = null, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}/volumes/{1}", GetServiceEndpoint(identity, region), volumeId));
            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.DELETE);

            return response != null && _validResponseCode.Contains(response.StatusCode);
        }

        public IEnumerable<VolumeType> ListVolumeTypes(string region = null, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}/types", GetServiceEndpoint(identity, region)));
            var response = ExecuteRESTRequest<ListVolumeTypeResponse>(identity, urlPath, HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return response.Data.VolumeTypes;
        }

        public VolumeType DescribeVolumeType(int volumeTypeId, string region = null, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}/types/{1}", GetServiceEndpoint(identity, region), volumeTypeId));
            var response = ExecuteRESTRequest<GetCloudBlockStorageVolumeTypeResponse>(identity, urlPath, HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return response.Data.VolumeType;
        }

        public Volume WaitForVolumeAvailable(string volumeId, int refreshCount = 600, int refreshDelayInMS = 2400, string region = null, CloudIdentity identity = null)
        {
            return WaitForVolumeState(volumeId, VolumeState.AVAILABLE, new[] { VolumeState.ERROR, VolumeState.ERROR_DELETING }, refreshCount, refreshDelayInMS, region, identity);
        }

        public bool WaitForVolumeDeleted(string volumeId, int refreshCount = 360, int refreshDelayInMS = 10000, string region = null, CloudIdentity identity = null)
        {
            return WaitForItemToBeDeleted(ShowVolume, volumeId, refreshCount, refreshDelayInMS, region, identity);
        }

        public Volume WaitForVolumeState(string volumeId, string expectedState, string[] errorStates, int refreshCount = 600, int refreshDelayInMS = 2400, string region = null, CloudIdentity identity = null)
        {
            var volumeInfo = ShowVolume(volumeId, region, identity);

            var count = 0;
            while (!volumeInfo.Status.Equals(expectedState, StringComparison.OrdinalIgnoreCase) && !errorStates.Contains(volumeInfo.Status) && count < refreshCount)
            {
                Thread.Sleep(refreshDelayInMS);
                volumeInfo = ShowVolume(volumeId, region, identity);
                count++;
            }

            if (errorStates.Contains(volumeInfo.Status))
                throw new VolumeEnteredErrorStateException(volumeInfo.Status);

            return volumeInfo;
        }

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

        public bool CreateSnapshot(string volumeId, bool force = false, string displayName = "None", string displayDescription = "None", string region = null, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}/snapshots", GetServiceEndpoint(identity, region)));
            var requestBody = new CreateCloudBlockStorageSnapshotRequest { CreateCloudBlockStorageSnapshotDetails = new CreateCloudBlockStorageSnapshotDetails { VolumeId = volumeId, Force = force, DisplayName = displayName, DisplayDescription = displayDescription } };
            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.POST, requestBody);

            return response != null && _validResponseCode.Contains(response.StatusCode);
        }

        public IEnumerable<Snapshot> ListSnapshots(string region = null, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}/snapshots", GetServiceEndpoint(identity, region)));
            var response = ExecuteRESTRequest<ListSnapshotResponse>(identity, urlPath, HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Snapshots;
        }

        public Snapshot ShowSnapshot(string snapshotId, string region = null, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}/snapshots/{1}", GetServiceEndpoint(identity, region), snapshotId));
            var response = ExecuteRESTRequest<GetCloudBlockStorageSnapshotResponse>(identity, urlPath, HttpMethod.GET);

            if (response == null || response.Data == null)
                return null;

            return response.Data.Snapshot;
        }

        public bool DeleteSnapshot(string snapshotId, string region = null, CloudIdentity identity = null)
        {
            var urlPath = new Uri(string.Format("{0}/snapshots/{1}", GetServiceEndpoint(identity, region), snapshotId));
            var response = ExecuteRESTRequest(identity, urlPath, HttpMethod.DELETE);

            return response != null && _validResponseCode.Contains(response.StatusCode);
        }

        public Snapshot WaitForSnapshotAvailable(string snapshotId, int refreshCount = 360, int refreshDelayInMS = 10000, string region = null, CloudIdentity identity = null)
        {
            return WaitForSnapshotState(snapshotId, SnapshotState.AVAILABLE, new[] { SnapshotState.ERROR, SnapshotState.ERROR_DELETING }, refreshCount, refreshDelayInMS, region, identity);
        }

        public bool WaitForSnapshotDeleted(string snapshotId, int refreshCount = 180, int refreshDelayInMS = 10000, string region = null, CloudIdentity identity = null)
        {
            return WaitForItemToBeDeleted(ShowSnapshot, snapshotId, refreshCount, refreshDelayInMS, region, identity);
        }

        public Snapshot WaitForSnapshotState(string snapshotId, string expectedState, string[] errorStates, int refreshCount = 60, int refreshDelayInMS = 10000, string region = null, CloudIdentity identity = null)
        {
            var snapshotInfo = ShowSnapshot(snapshotId, region, identity);

            var count = 0;
            while (!snapshotInfo.Status.Equals(expectedState, StringComparison.OrdinalIgnoreCase) && !errorStates.Contains(snapshotInfo.Status) && count < refreshCount)
            {
                Thread.Sleep(refreshDelayInMS);
                snapshotInfo = ShowSnapshot(snapshotId, region, identity);
                count++;
            }

            if (errorStates.Contains(snapshotInfo.Status))
                throw new SnapshotEnteredErrorStateException(snapshotInfo.Status);

            return snapshotInfo;
        }

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
            return base.GetPublicServiceEndpoint(identity, "cloudBlockStorage", region);
        }

        private bool WaitForItemToBeDeleted<T>(Func<string, string, CloudIdentity, T> retrieveItemMethod, string id, int refreshCount = 360, int refreshDelayInMS = 10000, string region = null, CloudIdentity identity = null)
        {
            try
            {
                retrieveItemMethod(id, region, identity);

                var count = 0;
                while (count < refreshCount)
                {
                    Thread.Sleep(refreshDelayInMS);
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

        #endregion
    }
}
