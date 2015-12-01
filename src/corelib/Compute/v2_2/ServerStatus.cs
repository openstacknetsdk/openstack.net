namespace OpenStack.Compute.v2_2
{
    /// <summary />
    public class ServerStatus : v2_1.ServerStatus
    {
        /// <inheritdoc />
        protected ServerStatus(string displayName)
            : base(displayName)
        { }

        /// <summary>
        /// The server is active.
        /// </summary>
        public new static readonly ServerStatus Active = new ServerStatus("ACTIVE");

        /// <summary>
        /// The server has not finished the original build process.
        /// </summary>
        public new static readonly ServerStatus Building = new ServerStatus("BUILDING");

        /// <summary>
        /// The server is permanently deleted.
        /// </summary>
        public new static readonly ServerStatus Deleted = new ServerStatus("DELETED");

        /// <summary>
        /// The server is in Error.
        /// </summary>
        public new static readonly ServerStatus Error = new ServerStatus("ERROR");

        /// <summary>
        /// The server is hard rebooting. This is equivalent to pulling the power plug on a physical server, plugging it back in, and rebooting it.
        /// </summary>
        public new static readonly ServerStatus HardReboot = new ServerStatus("HARD_REBOOT");

        /// <summary>
        /// The server is being migrated to a new host.
        /// </summary>
        public new static readonly ServerStatus Migrating = new ServerStatus("MIGRATING");

        /// <summary>
        /// The Password is being reset on the server.
        /// </summary>
        public new static readonly ServerStatus Password = new ServerStatus("PASSWORD");

        /// <summary>
        /// In a Paused state, the state of the server is stored in RAM.A Paused server continues to run in frozen state.
        /// </summary>
        public new static readonly ServerStatus Paused = new ServerStatus("PAUSED");

        /// <summary>
        /// The server is in a soft Reboot state. A Reboot command was passed to the operating system.
        /// </summary>
        public new static readonly ServerStatus Reboot = new ServerStatus("REBOOT");

        /// <summary>
        /// The server is currently being rebuilt from an image.
        /// </summary>
        public new static readonly ServerStatus Rebuild = new ServerStatus("REBUILD");

        /// <summary>
        /// The server is in rescue mode. A rescue image is running with the original server image attached.
        /// </summary>
        public new static readonly ServerStatus Rescued = new ServerStatus("RESCUED");

        /// <summary>
        /// Server is performing the differential copy of data that changed during its initial copy. Server is down for this stage.
        /// </summary>
        public new static readonly ServerStatus Resized = new ServerStatus("RESIZED");

        /// <summary>
        /// The resize or migration of a server failed for some reason. The destination server is being cleaned up and the original source server is restarting.
        /// </summary>
        public new static readonly ServerStatus RevertResize = new ServerStatus("REVERT_RESIZE");

        /// <summary>
        /// The server is marked as deleted but the disk images are still available to restore.
        /// </summary>
        public new static readonly ServerStatus SoftDeleted = new ServerStatus("SOFT_DELETED");

        /// <summary>
        /// The server is powered off and the disk image still persists.
        /// </summary>
        public new static readonly ServerStatus Stopped = new ServerStatus("STOPPED");

        /// <summary>
        /// The server is suspended, either by request or necessity.
        /// </summary>
        public new static readonly ServerStatus Suspended = new ServerStatus("SUSPENDED");

        /// <summary>
        /// The state of the server is unknown.Contact your cloud provider.
        /// </summary>
        public new static readonly ServerStatus Unknown = new ServerStatus("UNKNOWN");

        /// <summary>
        /// System is awaiting confirmation that the server is operational after a move or resize.
        /// </summary>
        public new static readonly ServerStatus VerifyResize = new ServerStatus("VERIFY_RESIZE");

    }
}