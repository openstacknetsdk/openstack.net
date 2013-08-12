using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using net.openstack.Core.Providers;

namespace net.openstack.Core.Domain
{
    [DataContract]
    public abstract class ServerBase : ProviderStateBase<IComputeProvider>
    {
        [DataMember]
        public string Id { get; internal set; }

        [DataMember]
        public Link[] Links { get; internal set; }

        protected virtual void UpdateThis(ServerBase server)
        {
            if (server == null)
                return;

            Id = server.Id;
            Links = server.Links;
        }

        /// <summary>
        /// Waits for the server to enter the ACTIVE state
        /// </summary>
        /// <param name="refreshCount">Number of times to check the images status</param>
        /// <param name="refreshDelay">The time to wait each time before requesting the status for the image. If this value is <c>null</c>, the default is 2.4 seconds.</param>
        /// <param name="progressUpdatedCallback">A callback delegate to execute each time the <see cref="SimpleServer"/>s Progress value increases.</param>
        public void WaitForActive(int refreshCount = 600, TimeSpan? refreshDelay = null, Action<int> progressUpdatedCallback = null)
        {
            var details = Provider.WaitForServerActive(Id, refreshCount, refreshDelay ?? TimeSpan.FromMilliseconds(2400), progressUpdatedCallback, Region);
            UpdateThis(details);
        }

        /// <summary>
        /// Waits for the server to enter the DELETED state
        /// </summary>
        /// <param name="refreshCount">Number of times to check the server status</param>
        /// <param name="refreshDelay">The time to wait each time before requesting the status for the server. If this value is <c>null</c>, the default is 2.4 seconds.</param>
        /// <param name="progressUpdatedCallback">A callback delegate to execute each time the <see cref="SimpleServer"/>s Progress value increases.</param>
        public void WaitForDeleted(int refreshCount = 600, TimeSpan? refreshDelay = null, Action<int> progressUpdatedCallback = null)
        {
            Provider.WaitForServerDeleted(Id, refreshCount, refreshDelay ?? TimeSpan.FromMilliseconds(2400), progressUpdatedCallback, Region);
        }

        /// <summary>
        /// Waits for the server to enter a particular <see cref="ServerState"/>
        /// </summary>
        /// <param name="expectedState">The expected <see cref="ServerState"/></param>
        /// <param name="errorStates">A list of <see cref="ServerState"/>s in which to throw an exception if the server enters. </param>
        /// <param name="refreshCount">Number of times to check the server status</param>
        /// <param name="refreshDelay">The time to wait each time before requesting the status for the server. If this value is <c>null</c>, the default is 2.4 seconds.</param>
        /// <param name="progressUpdatedCallback">A callback delegate to execute each time the <see cref="SimpleServer"/>s Progress value increases.</param>
        public void WaitForState(string expectedState, string[] errorStates, int refreshCount = 600, TimeSpan? refreshDelay = null, Action<int> progressUpdatedCallback = null)
        {
            var details = Provider.WaitForServerState(Id, expectedState, errorStates, refreshCount, refreshDelay ?? TimeSpan.FromMilliseconds(2400), progressUpdatedCallback, Region);
            UpdateThis(details);
        }

        /// <summary>
        /// Waits for the server to enter a particular <see cref="ServerState"/>
        /// </summary>
        /// <param name="expectedStates">The set expected <see cref="ServerState"/>s</param>
        /// <param name="errorStates">A list of <see cref="ServerState"/>s in which to throw an exception if the server enters. </param>
        /// <param name="refreshCount">Number of times to check the server status</param>
        /// <param name="refreshDelay">The time to wait each time before requesting the status for the server. If this value is <c>null</c>, the default is 2.4 seconds.</param>
        /// <param name="progressUpdatedCallback">A callback delegate to execute each time the <see cref="SimpleServer"/>s Progress value increases.</param>
        public void WaitForState(string[] expectedStates, string[] errorStates, int refreshCount = 600, TimeSpan? refreshDelay = null, Action<int> progressUpdatedCallback = null)
        {
            var details = Provider.WaitForServerState(Id, expectedStates, errorStates, refreshCount, refreshDelay ?? TimeSpan.FromMilliseconds(2400), progressUpdatedCallback, Region);
            UpdateThis(details);
        }

        /// <summary>
        /// Initiates a soft reboot of the server
        /// </summary>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        public bool SoftReboot()
        {
            return Provider.RebootServer(Id, RebootType.Soft, Region);
        }

        /// <summary>
        /// Initiates a hard reboot of the server
        /// </summary>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        public bool HardReboot()
        {
            return Provider.RebootServer(Id, RebootType.Hard, Region);
        }

        /// <summary>
        /// Initiates a rebuild of the server
        /// </summary>
        /// <param name="name">The new name for the server.</param>
        /// <param name="imageId">The ID of the new image for the server.</param>
        /// <param name="flavor">The new flavor for server.</param>
        /// <param name="adminPassword">The new admin password for the server.</param>
        /// <param name="accessIPv4">The new access IP v4 address for the server. </param>
        /// <param name="accessIPv6">The new access IP v6 address for the server. </param>
        /// <param name="metadata">The list of any metadata to associate with the server. </param>
        /// <param name="diskConfig">The disk configuration value. <remarks>Available values are [AUTO, MANUAL]</remarks></param>
        /// <param name="personality">The path and contents of a file to inject in the target file system during the rebuild operation. If the value is <c>null</c>, no file is injected.</param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        public bool Rebuild(string name, string imageId, string flavor, string adminPassword, string accessIPv4 = null, string accessIPv6 = null, Metadata metadata = null, string diskConfig = null, Personality personality = null)
        {
            var details = Provider.RebuildServer(Id, name, imageId, flavor, adminPassword, accessIPv4, accessIPv6, metadata, diskConfig, personality, Region);

            if (details == null)
                return false;

            UpdateThis(details);

            return true;
        }

        /// <summary>
        /// Initiates a resize of the server.
        /// </summary>
        /// <param name="name">The new name for the resized server.</param>
        /// <param name="flavor">The new flavor.</param>
        /// <param name="diskConfig">The disk configuration value. <remarks>Available values are {AUTO|MANUAL}</remarks></param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        public bool Resize(string name, string flavor, string diskConfig = null)
        {
            return Provider.ResizeServer(Id, name, flavor, diskConfig, Region);
        }

        /// <summary>
        /// Confirms a server resize.
        /// </summary>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        public bool ConfirmResize()
        {
            return Provider.ConfirmServerResize(Id, Region);
        }

        /// <summary>
        /// Reverts a server resize.
        /// </summary>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        public bool RevertResize()
        {
            return Provider.RevertServerResize(Id, Region);
        }

        /// <summary>
        /// Puts the server into rescue mode
        /// </summary>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        public string Rescue()
        {
            return Provider.RescueServer(Id, Region);
        }

        /// <summary>
        /// Removes the server from Rescue state.
        /// </summary>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        public bool UnRescue()
        {
            return Provider.UnRescueServer(Id, Region);
        }

        /// <summary>
        /// Creates a snapshot of server's current state.
        /// </summary>
        /// <param name="imageName"></param>
        /// <param name="metadata"></param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        public bool CreateSnapshot(string imageName, Metadata metadata = null)
        {
            return Provider.CreateImage(Id, imageName, metadata, Region);
        }

        /// <summary>
        /// Return full details of the <see cref="Server"/>
        /// </summary>
        /// <returns><see cref="Server"/></returns>
        public Server GetDetails()
        {
            return Provider.GetDetails(Id, Region);
        }

        /// <summary>
        /// Lists all volumes attached to ther server.
        /// </summary>
        /// <returns>a list of <see cref="ServerVolume"/>s</returns>
        public IEnumerable<ServerVolume> ListVolumes()
        {
            return Provider.ListServerVolumes(Id, Region);
        }

        /// <summary>
        /// Attaches a volume to the server.
        /// </summary>
        /// <param name="volumeId">The volume ID.</param>
        /// <param name="storageDevice">The name of the device, such as /dev/xvdb. <remarks>If null, this value will be auto assigned</remarks></param>
        /// <returns></returns>
        public ServerVolume AttachVolume(string volumeId, string storageDevice)
        {
            return Provider.AttachServerVolume(Id, volumeId, storageDevice, Region);
        }

        /// <summary>
        /// Detaches a volume from the server
        /// </summary>
        /// <param name="volumeId">The volume ID.</param>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        public bool DetachVolume(string volumeId)
        {
            return Provider.DetachServerVolume(Id, volumeId, Region);
        }

        /// <summary>
        /// Updates the server object values.
        /// </summary>
        public void Refresh()
        {
            var details = this.GetDetails();

            UpdateThis(details);
        }
    }
}