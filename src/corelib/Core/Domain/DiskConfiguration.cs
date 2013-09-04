namespace net.openstack.Core.Domain
{
    /// <summary>
    /// This enumeration is part of the <see href="http://docs.rackspace.com/servers/api/v2/cs-devguide/content/ch_extensions.html#diskconfig_attribute"><newTerm>disk configuration extension</newTerm></see>,
    /// which adds at attribute to images and servers to control how the disk is partitioned when
    /// servers are created, rebuilt, or resized.
    /// </summary>
    /// <seealso href="http://docs.rackspace.com/servers/api/v2/cs-devguide/content/ch_extensions.html#diskconfig_attribute">Disk Configuration Extension (Rackspace Next Generation Cloud Servers Developer Guide - API v2)</seealso>
    public enum DiskConfiguration
    {
        /// <summary>
        /// The server is built with a single partition the size of the target flavor disk. The
        /// file system is automatically adjusted to fit the entire partition. This keeps things
        /// simple and automated. <see cref="Auto"/> is valid only for images and servers with a
        /// single partition that use the EXT3 file system. This is the default setting for
        /// applicable Rackspace base images.
        /// </summary>
        Auto,

        /// <summary>
        /// The server is built using whatever partition scheme and file system is in the source
        /// image. If the target flavor disk is larger, the remaining disk space is left
        /// unpartitioned. This enables images to have non-EXT3 file systems, multiple partitions,
        /// and so on, and enables you to manage the disk configuration.
        /// </summary>
        Manual,
    }
}
