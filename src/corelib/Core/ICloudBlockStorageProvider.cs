using System;
using System.Collections.Generic;
using net.openstack.Core.Domain;

namespace net.openstack.Core
{
    public interface ICloudBlockStorageProvider
    {
        #region Volume
        /// <summary>
        /// Creates a new volume.
        /// 
        /// Documentation URL: http://docs.rackspace.com/cbs/api/v1.0/cbs-devguide/content/POST_createVolume__v1__tenant_id__volumes.html
        /// </summary>
        /// <param name="size">The size (in GB) of the volume.  The size parameter should always be supplied and should be between 100 and 1000.</param>
        /// <param name="display_description">A description of the volume.<remarks>[Optional]</remarks></param>
        /// <param name="display_name">The name of the volume.<remarks>[Optional]</remarks></param>
        /// <param name="snapshot_id">The optional snapshot from which to create a volume.<remarks>[Optional]</remarks></param>
        /// <param name="volume_type">The type of volume to create, either SATA or SSD. If not defined, then the default, SATA, is used.<remarks>[Optional]</remarks></param>
        /// <param name="region">The region in which to build the server.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <exception cref="net.openstack.Providers.Rackspace.Exceptions.InvalidVolumeSizeException"></exception>
        /// <returns><see cref="bool"></see></returns>
        bool CreateVolume(int size, string display_description = null, string display_name = null, string snapshot_id = null, string volume_type = null, string region = null, CloudIdentity identity = null);
        /// <summary>
        /// View a list of volumes.
        /// 
        /// Documenatation URL: http://docs.rackspace.com/cbs/api/v1.0/cbs-devguide/content/GET_getVolumesSimple__v1__tenant_id__volumes.html
        /// </summary>
        /// <param name="region">The region in which to build the server.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>List of <see cref="net.openstack.Core.Domain.Volume"></see> objects.</returns>
        IEnumerable<Volume> ListVolumes(string region = null, CloudIdentity identity = null);
        /// <summary>
        /// View all information about a single volume.
        /// 
        /// Documenatation URL: http://docs.rackspace.com/cbs/api/v1.0/cbs-devguide/content/GET_getVolume__v1__tenant_id__volumes.html
        /// </summary>
        /// <param name="volume_id">The ID of the volume</param>
        /// <param name="region">The region in which to build the server.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><see cref="net.openstack.Core.Domain.Volume"></see></returns>
        Volume ShowVolume(string volume_id, string region = null, CloudIdentity identity = null);
        /// <summary>
        /// Deletes a volume.
        /// 
        /// NOTE: 
        /// It is not currently possible to delete a volume once you have created a snapshot from it.  Any snapshots will need to be deleted prior to deleting the Volume.
        /// 
        /// Documentation URL: http://docs.rackspace.com/cbs/api/v1.0/cbs-devguide/content/DELETE_deleteVolume__v1__tenant_id__volumes.html
        /// </summary>
        /// <param name="volume_id">The ID of the volume.</param>
        /// <param name="region">The region in which to build the server.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><see cref="bool"></see></returns>
        bool DeleteVolume(string volume_id, string region = null, CloudIdentity identity = null);
        /// <summary>
        /// Request a list of volume types.
        /// 
        /// Documentation URL: http://docs.rackspace.com/cbs/api/v1.0/cbs-devguide/content/GET_getVolumeTypes__v1__tenant_id__types.html
        /// </summary>
        /// <param name="region">The region in which to build the server.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>List of <see cref="net.openstack.Core.Domain.VolumeType"></see> objects. </returns>
        IEnumerable<VolumeType> ListVolumeTypes(string region = null, CloudIdentity identity = null);
        /// <summary>
        /// Request a single volume type.
        /// 
        /// Documentation URL: http://docs.rackspace.com/cbs/api/v1.0/cbs-devguide/content/GET_getVolumeType__v1__tenant_id__types.html
        /// </summary>
        /// <param name="volume_type_id">The ID of the volume type.</param>
        /// <param name="region">The region in which to build the server.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><see cref="net.openstack.Core.Domain.VolumeType"></see></returns>
        VolumeType DescribeVolumeType(int volume_type_id, string region = null, CloudIdentity identity = null);
        /// <summary>
        /// Waits for a volume to be set to "available" status.  
        /// This method is designed to wait for a volume to move to "available" status after the CreateVolume method is called.  This method
        /// will be helpful to ensure that a volume is correctly created prior to executing additional requests against it.
        /// </summary>
        /// <param name="volume_id">The ID of the volume to poll.</param>
        /// <param name="refreshCount">The number of times to poll for the volume to become "available".</param>
        /// <param name="refreshDelayInMS">The refresh delay in milliseconds.</param>
        /// <param name="region">The region in which to build the server.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><see cref="net.openstack.Core.Domain.Volume"></see></returns>
        Volume WaitForVolumeAvailable(string volume_id, int refreshCount = 600, int refreshDelayInMS = 2400, string region = null, CloudIdentity identity = null);
        /// <summary>
        /// Waits for a volume to be deleted.  
        /// This method is designed to wait for a volume to be completely deleted after the DeleteVolume method is called.  
        /// This method will be helpful to ensure that a volume is completely removed.
        /// </summary>
        /// <param name="volume_id">The ID of the volume to poll.</param>
        /// <param name="refreshCount">The number of times to poll for the volume to be deleted.</param>
        /// <param name="refreshDelayInMS">The refresh delay in milliseconds.</param>
        /// <param name="region">The region in which to build the server.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><see cref="bool"></see></returns>
        bool WaitForVolumeDeleted(string volume_id, int refreshCount = 360, int refreshDelayInMS = 10000, string region = null, CloudIdentity identity = null);
        /// <summary>
        /// Waits for a volume to be set to be set to a particular status.  
        /// This method is designed to wait for a volume to move to a particular status.  
        /// This method will be helpful to ensure that a volume is in an intended state prior to executing additional requests against it.
        /// 
        /// Volume State reference URL:  http://docs.rackspace.com/cbs/api/v1.0/cbs-devguide/content/volume_status.html
        /// <see cref="net.openstack.Core.Domain.VolumeState"></see>
        /// </summary>
        /// <param name="volume_id">The ID of the volume to poll.</param>
        /// <param name="expectedState">The expected state for the volume.</param>
        /// <param name="errorStates">The error state(s) in which to stop polling once reached.</param>
        /// <param name="refreshCount">The number of times to poll the volume.</param>
        /// <param name="refreshDelayInMS">The refresh delay in milliseconds.</param>
        /// <param name="region">The region in which to build the server.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><see cref="net.openstack.Core.Domain.Volume"></see></returns>
        /// <exception cref="net.openstack.Providers.Rackspace.CloudBlockStorageProvider.VolumeEnteredErrorStateException"></exception>
        Volume WaitForVolumeState(string volume_id, string expectedState, string[] errorStates, int refreshCount = 600, int refreshDelayInMS = 2400, string region = null, CloudIdentity identity = null);
        #endregion

        #region Snapshot
        /// <summary>
        /// Creates a new snapshot.
        /// 
        /// Creating a snapshot makes a point-in-time copy of the volume. 
        /// All writes to the volume should be flushed before creating the snapshot, either by un-mounting any file systems on the volume, or by detaching the volume before creating the snapshot. 
        /// Snapshots are incremental, so each time you create a new snapshot, you are appending the incremental changes for the new snapshot to the previous one. 
        /// The previous snapshot is still available. Note that you can create a new volume from the snapshot if desired.

        /// Documentation URL: http://docs.rackspace.com/cbs/api/v1.0/cbs-devguide/content/POST_createSnapshot__v1__tenant_id__snapshots.html
        /// </summary>
        /// <param name="volume_id">The ID of the volume to snapshot.</param>
        /// <param name="force">Indicates whether to snapshot, even if the volume is attached. Default==False.<remarks>[Optional]</remarks></param>
        /// <param name="display_name">Name of the snapshot. Default==None. <remarks>[Optional]</remarks></param>
        /// <param name="display_description">Description of snapshot. Default==None.<remarks>[Optional]</remarks></param>
        /// <param name="region">The region in which to build the server.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><see cref="bool"></see></returns>
        bool CreateSnapshot(string volume_id, bool force = false, string display_name = "None", string display_description = null, string region = null, CloudIdentity identity = null);
        /// <summary>
        /// View a list of snapshots.
        /// 
        /// Documenatation URL: http://docs.rackspace.com/cbs/api/v1.0/cbs-devguide/content/GET_getSnapshotsSimple__v1__tenant_id__snapshots.html
        /// </summary>
        /// <param name="region">The region in which to build the server.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns>List of <see cref="net.openstack.Core.Domain.Snapshot"></see> objects.</returns>
        IEnumerable<Snapshot> ListSnapshots(string region = null, CloudIdentity identity = null);
        /// <summary>
        /// View all information about a single snapshot.
        /// 
        /// Documenatation URL: http://docs.rackspace.com/cbs/api/v1.0/cbs-devguide/content/GET_getSnapshot__v1__tenant_id__snapshots.html
        /// </summary>
        /// <param name="snapshot_id">The ID of the snapshot</param>
        /// <param name="region">The region in which to build the server.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><see cref="net.openstack.Core.Domain.Snapshot"></see></returns>
        Snapshot ShowSnapshot(string snapshot_id, string region = null, CloudIdentity identity = null);
        /// <summary>
        /// Deletes a single snapshot.
        /// 
        /// Documentation URL: http://docs.rackspace.com/cbs/api/v1.0/cbs-devguide/content/DELETE_deleteSnapshot__v1__tenant_id__snapshots.html
        /// </summary>
        /// <param name="snapshot_id">The ID of the snapshot.</param>
        /// <param name="region">The region in which to build the server.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><see cref="bool"></see></returns>
        bool DeleteSnapshot(string snapshot_id, string region = null, CloudIdentity identity = null);
        /// <summary>
        /// Waits for a snapshot to be set to "available" status.  
        /// This method is designed to wait for a snapshot to move to "available" status after the CreateSnapshot method is called.  
        /// This method will be helpful to ensure that a snapshot is correctly created prior to executing additional requests against it.
        /// </summary>
        /// <param name="snapshot_id">The ID of the snapshot to poll.</param>
        /// <param name="refreshCount">The number of times to poll for the snapshot to become "available".</param>
        /// <param name="refreshDelayInMS">The refresh delay in milliseconds.</param>
        /// <param name="region">The region in which to build the server.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><see cref="net.openstack.Core.Domain.Snapshot"></see></returns>
        Snapshot WaitForSnapshotAvailable(string snapshot_id, int refreshCount = 360, int refreshDelayInMS = 10000, string region = null, CloudIdentity identity = null);
        /// <summary>
        /// Waits for a snapshot to be deleted.  
        /// This method is designed to wait for a snapshot to be completely deleted after the DeleteSnapshot method is called.  
        /// This method will be helpful to ensure that a snapshot is completely removed.
        /// </summary>
        /// <param name="snapshot_id">The ID of the snapshot to poll.</param>
        /// <param name="refreshCount">The number of times to poll for the snapshot to be deleted.</param>
        /// <param name="refreshDelayInMS">The refresh delay in milliseconds.</param>
        /// <param name="region">The region in which to build the server.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><see cref="bool"></see></returns>
        bool WaitForSnapshotDeleted(string snapshot_id, int refreshCount = 180, int refreshDelayInMS = 10000, string region = null, CloudIdentity identity = null);
        /// <summary>
        /// Waits for a snapshot to be set to be set to a particular status.  
        /// This method is designed to wait for a snapshot to move to a particular status.  
        /// This method will be helpful to ensure that a snapshot is in an intended state prior to executing additional requests against it.
        /// 
        /// <see cref="net.openstack.Core.Domain.SnapshotState"></see> 
        /// </summary>
        /// <param name="snapshot_id">The ID of the snapshot to poll.</param>
        /// <param name="expectedState">The expected state for the snapshot.</param>
        /// <param name="errorStates">The error state(s) in which to stop polling once reached.</param>
        /// <param name="refreshCount">The number of times to poll the snapshot.</param>
        /// <param name="refreshDelayInMS">The refresh delay in milliseconds.</param>
        /// <param name="region">The region in which to build the server.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity <see cref="net.openstack.Core.Domain.CloudIdentity"/><remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks></param>
        /// <returns><see cref="net.openstack.Core.Domain.Snapshot"></see></returns>
        /// <exception cref="net.openstack.Providers.Rackspace.CloudBlockStorageProvider.SnapshotEnteredErrorStateException"></exception>
        Snapshot WaitForSnapshotState(string snapshot_id, string expectedState, string[] errorStates, int refreshCount = 60, int refreshDelayInMS = 10000, string region = null, CloudIdentity identity = null);
        #endregion
    }
}
