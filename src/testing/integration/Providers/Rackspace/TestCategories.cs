﻿namespace Net.OpenStack.Testing.Integration.Providers.Rackspace
{
    using net.openstack.Core.Providers;
    using net.openstack.Core.Synchronous;

    /// <summary>
    /// Provides predefined categories for use with the <see cref="TestCategoryAttribute"/>.
    /// </summary>
    public static class TestCategories
    {
        /// <summary>
        /// Identity service tests.
        /// </summary>
        /// <seealso cref="IIdentityProvider"/>
        public const string Identity = "Identity";

        /// <summary>
        /// Block storage service tests.
        /// </summary>
        /// <seealso cref="IBlockStorageProvider"/>
        public const string BlockStorage = "BlockStorage";

        /// <summary>
        /// Object storage service tests.
        /// </summary>
        /// <seealso cref="IObjectStorageProvider"/>
        public const string ObjectStorage = "ObjectStorage";

        /// <summary>
        /// Networks service tests.
        /// </summary>
        /// <seealso cref="INetworksProvider"/>
        public const string Networks = "Networks";

        /// <summary>
        /// Compute service tests.
        /// </summary>
        /// <seealso cref="IComputeProvider"/>
        public const string Compute = "Compute";

        /// <summary>
        /// DNS service tests.
        /// </summary>
        /// <seealso cref="IDnsService"/>
        public const string Dns = "DNS";

        /// <summary>
        /// Load balancer service tests.
        /// </summary>
        /// <seealso cref="ILoadBalancerProvider"/>
        /// <preliminary/>
        public const string LoadBalancers = "LoadBalancers";

        /// <summary>
        /// Queueing service tests.
        /// </summary>
        /// <seealso cref="IQueueingService"/>
        public const string Queues = "Queues";

        /// <summary>
        /// Tests Synchronous extensions to the queueing service.
        /// </summary>
        /// <seealso cref="QueueingServiceExtensions"/>
        public const string QueuesSynchronous = "QueuesSynchronous";

        /// <summary>
        /// Unit tests designed to remove resources from an account which were created
        /// by previous unit test runs which were cancelled, failed, or designed in such
        /// a way that resources were not deleted automatically at the end of the test.
        /// </summary>
        public const string Cleanup = "Cleanup";

        /// <summary>
        /// Tests which require user credentials.
        /// </summary>
        public const string User = "User";

        /// <summary>
        /// Tests which require administrator credentials.
        /// </summary>
        public const string Admin = "Admin";
    }
}
