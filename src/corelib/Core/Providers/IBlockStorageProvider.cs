using System;
using System.Collections.Generic;
using net.openstack.Core.Domain;

namespace net.openstack.Core.Providers
{
    /// <summary>
    /// Provides simple access to the Rackspace Cloud Block Storage Volumes as well as Cloud Block Storage Volume Snapshot services.
    /// </summary>
    public interface IBlockStorageProvider
    {
        #region Volume
        /// <summary>
        /// Creates a new volume.
        /// <para/>
        /// Documentation URL: http://docs.rackspace.com/cbs/api/v1.0/cbs-devguide/content/POST_createVolume__v1__tenant_id__volumes.html
        /// </summary>
        /// <param name="size">The size (in GB) of the volume.  The size parameter should always be supplied and should be between 100 and 1000.</param>
        /// <param name="displayDescription">A description of the volume.</param>
        /// <param name="displayName">The name of the volume.</param>
        /// <param name="snapshotId">The optional snapshot from which to create a volume.</param>
        /// <param name="volumeType">The type of volume to create, either SATA or SSD. If not defined, then the default, SATA, is used.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <exception cref="net.openstack.Providers.Rackspace.Exceptions.InvalidVolumeSizeException"></exception>
        /// <returns><see cref="bool"></see></returns>
        bool CreateVolume(int size, string displayDescription = null, string displayName = null, string snapshotId = null, string volumeType = null, string region = null, CloudIdentity identity = null);
        /// <summary>
        /// View a list of volumes.
        /// <para/>
        /// Documenatation URL: http://docs.rackspace.com/cbs/api/v1.0/cbs-devguide/content/GET_getVolumesSimple__v1__tenant_id__volumes.html
        /// </summary>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>List of <see cref="net.openstack.Core.Domain.Volume"></see> objects.</returns>
        IEnumerable<Volume> ListVolumes(string region = null, CloudIdentity identity = null);
        /// <summary>
        /// View information about a single volume.
        /// <para/>
        /// Documenatation URL: http://docs.rackspace.com/cbs/api/v1.0/cbs-devguide/content/GET_getVolume__v1__tenant_id__volumes.html
        /// </summary>
        /// <param name="volumeId">The ID of the volume</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><see cref="net.openstack.Core.Domain.Volume"></see></returns>
        Volume ShowVolume(string volumeId, string region = null, CloudIdentity identity = null);
        /// <summary>
        /// Deletes a volume.
        /// <para/>
        /// NOTE:  It is not currently possible to delete a volume once you have created a snapshot from it.  Any snapshots will need to be deleted prior to deleting the Volume.
        /// <para/>
        /// Documentation URL: http://docs.rackspace.com/cbs/api/v1.0/cbs-devguide/content/DELETE_deleteVolume__v1__tenant_id__volumes.html
        /// </summary>
        /// <param name="volumeId">The ID of the volume.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><see cref="bool"></see></returns>
        bool DeleteVolume(string volumeId, string region = null, CloudIdentity identity = null);
        /// <summary>
        /// Request a list of volume types.
        /// <para/>
        /// Documentation URL: http://docs.rackspace.com/cbs/api/v1.0/cbs-devguide/content/GET_getVolumeTypes__v1__tenant_id__types.html
        /// </summary>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>List of <see cref="net.openstack.Core.Domain.VolumeType"></see> objects. </returns>
        IEnumerable<VolumeType> ListVolumeTypes(string region = null, CloudIdentity identity = null);
        /// <summary>
        /// View information about a single volume type.
        /// <para/>
        /// Documentation URL: http://docs.rackspace.com/cbs/api/v1.0/cbs-devguide/content/GET_getVolumeType__v1__tenant_id__types.html
        /// </summary>
        /// <param name="volumeTypeId">The ID of the volume type.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><see cref="net.openstack.Core.Domain.VolumeType"></see></returns>
        VolumeType DescribeVolumeType(string volumeTypeId, string region = null, CloudIdentity identity = null);
        /// <summary>
        /// Waits for a volume to be set to <see cref="VolumeState.Available"/> status.  
        /// This method will be helpful to ensure that a volume is correctly created prior to executing additional requests against it.
        /// </summary>
        /// <param name="volumeId">The ID of the volume to poll.</param>
        /// <param name="refreshCount">The number of times to poll for the volume to become "available".</param>
        /// <param name="refreshDelayInMS">The refresh delay. If the value is <c>null</c>, the default value is 2.4 seconds.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><see cref="net.openstack.Core.Domain.Volume"></see></returns>
        Volume WaitForVolumeAvailable(string volumeId, int refreshCount = 600, TimeSpan? refreshDelay = null, string region = null, CloudIdentity identity = null);
        /// <summary>
        /// Waits for a volume to be deleted.  
        /// This method will be helpful to ensure that a volume is completely removed.
        /// </summary>
        /// <param name="volumeId">The ID of the volume to poll.</param>
        /// <param name="refreshCount">The number of times to poll for the volume to be deleted.</param>
        /// <param name="refreshDelayInMS">The refresh delay. If the value is <c>null</c>, the default value is 10 seconds.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><see cref="bool"></see></returns>
        bool WaitForVolumeDeleted(string volumeId, int refreshCount = 360, TimeSpan? refreshDelay = null, string region = null, CloudIdentity identity = null);
        /// <summary>
        /// Waits for a volume to be set to be set to a particular status.  
        /// This method will be helpful to ensure that a volume is in an intended state prior to executing additional requests against it.
        /// <para/>
        /// Volume State reference URL:  http://docs.rackspace.com/cbs/api/v1.0/cbs-devguide/content/volume_status.html
        /// <see cref="net.openstack.Core.Domain.VolumeState"></see>
        /// </summary>
        /// <param name="volumeId">The ID of the volume to poll.</param>
        /// <param name="expectedState">The expected state for the volume.</param>
        /// <param name="errorStates">The error state(s) in which to stop polling once reached.</param>
        /// <param name="refreshCount">The number of times to poll the volume.</param>
        /// <param name="refreshDelayInMS">The refresh delay. If the value is <c>null</c>, the default value is 10 seconds.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><see cref="net.openstack.Core.Domain.Volume"></see></returns>
        /// <exception cref="net.openstack.Providers.Rackspace.CloudBlockStorageProvider.VolumeEnteredErrorStateException"></exception>
        Volume WaitForVolumeState(string volumeId, VolumeState expectedState, VolumeState[] errorStates, int refreshCount = 600, TimeSpan? refreshDelay = null, string region = null, CloudIdentity identity = null);
        #endregion

        #region Snapshot
        /// <summary>
        /// Creates a new snapshot.
        /// <para/>
        /// Creating a snapshot makes a point-in-time copy of the volume. 
        /// All writes to the volume should be flushed before creating the snapshot, either by un-mounting any file systems on the volume, or by detaching the volume before creating the snapshot. 
        /// Snapshots are incremental, so each time you create a new snapshot, you are appending the incremental changes for the new snapshot to the previous one. 
        /// The previous snapshot is still available. Note that you can create a new volume from the snapshot if desired.
        /// <para/>
        /// Documentation URL: http://docs.rackspace.com/cbs/api/v1.0/cbs-devguide/content/POST_createSnapshot__v1__tenant_id__snapshots.html
        /// </summary>
        /// <param name="volumeId">The ID of the volume to snapshot.</param>
        /// <param name="force">Indicates whether to snapshot, even if the volume is attached. Default==False.</param>
        /// <param name="displayName">Name of the snapshot. Default==None. </param>
        /// <param name="displayDescription">Description of snapshot. Default==None.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><see cref="bool"></see></returns>
        bool CreateSnapshot(string volumeId, bool force = false, string displayName = "None", string displayDescription = null, string region = null, CloudIdentity identity = null);
        /// <summary>
        /// View a list of snapshots.
        /// <para/>
        /// Documenatation URL: http://docs.rackspace.com/cbs/api/v1.0/cbs-devguide/content/GET_getSnapshotsSimple__v1__tenant_id__snapshots.html
        /// </summary>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>List of <see cref="net.openstack.Core.Domain.Snapshot"></see> objects.</returns>
        IEnumerable<Snapshot> ListSnapshots(string region = null, CloudIdentity identity = null);
        /// <summary>
        /// View all information about a single snapshot.
        /// <para/>
        /// Documenatation URL: http://docs.rackspace.com/cbs/api/v1.0/cbs-devguide/content/GET_getSnapshot__v1__tenant_id__snapshots.html
        /// </summary>
        /// <param name="snapshotId">The ID of the snapshot</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><see cref="net.openstack.Core.Domain.Snapshot"></see></returns>
        Snapshot ShowSnapshot(string snapshotId, string region = null, CloudIdentity identity = null);
        /// <summary>
        /// Deletes a single snapshot.
        /// <para/>
        /// Documentation URL: http://docs.rackspace.com/cbs/api/v1.0/cbs-devguide/content/DELETE_deleteSnapshot__v1__tenant_id__snapshots.html
        /// </summary>
        /// <param name="snapshotId">The ID of the snapshot.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><see cref="bool"></see></returns>
        bool DeleteSnapshot(string snapshotId, string region = null, CloudIdentity identity = null);
        /// <summary>
        /// Waits for a snapshot to be set to <see cref="SnapshotState.Available"/> status.
        /// This method will be helpful to ensure that a snapshot is correctly created prior to executing additional requests against it.
        /// </summary>
        /// <param name="snapshotId">The ID of the snapshot to poll.</param>
        /// <param name="refreshCount">The number of times to poll for the snapshot to become "available".</param>
        /// <param name="refreshDelay">The refresh delay. If the value is <c>null</c>, the default value is 10 seconds.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><see cref="net.openstack.Core.Domain.Snapshot"></see></returns>
        Snapshot WaitForSnapshotAvailable(string snapshotId, int refreshCount = 360, TimeSpan? refreshDelay = null, string region = null, CloudIdentity identity = null);
        /// <summary>
        /// Waits for a snapshot to be deleted.  
        /// This method will be helpful to ensure that a snapshot is completely removed.
        /// </summary>
        /// <param name="snapshotId">The ID of the snapshot to poll.</param>
        /// <param name="refreshCount">The number of times to poll for the snapshot to be deleted.</param>
        /// <param name="refreshDelay">The refresh delay. If the value is <c>null</c>, the default value is 10 seconds.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><see cref="bool"></see></returns>
        bool WaitForSnapshotDeleted(string snapshotId, int refreshCount = 180, TimeSpan? refreshDelay = null, string region = null, CloudIdentity identity = null);
        /// <summary>
        /// Waits for a snapshot to be set to be set to a particular status.  
        /// This method will be helpful to ensure that a snapshot is in an intended state prior to executing additional requests against it.
        /// 
        /// <see cref="net.openstack.Core.Domain.SnapshotState"></see> 
        /// </summary>
        /// <param name="snapshotId">The ID of the snapshot to poll.</param>
        /// <param name="expectedState">The expected state for the snapshot.</param>
        /// <param name="errorStates">The error state(s) in which to stop polling once reached.</param>
        /// <param name="refreshCount">The number of times to poll the snapshot.</param>
        /// <param name="refreshDelay">The refresh delay. If the value is <c>null</c>, the default value is 10 seconds.</param>
        /// <param name="region">The region in which to execute this action.<remarks>If not specified, the user’s default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><see cref="net.openstack.Core.Domain.Snapshot"></see></returns>
        /// <exception cref="net.openstack.Providers.Rackspace.CloudBlockStorageProvider.SnapshotEnteredErrorStateException"></exception>
        Snapshot WaitForSnapshotState(string snapshotId, SnapshotState expectedState, SnapshotState[] errorStates, int refreshCount = 60, TimeSpan? refreshDelay = null, string region = null, CloudIdentity identity = null);
        #endregion
    }
}
