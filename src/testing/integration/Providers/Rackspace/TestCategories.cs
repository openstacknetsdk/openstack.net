namespace Net.OpenStack.Testing.Integration.Providers.Rackspace
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
        public const string BlockStorage = "Block Storage";

        /// <summary>
        /// Object storage service tests.
        /// </summary>
        /// <seealso cref="IObjectStorageProvider"/>
        public const string ObjectStorage = "Object Storage";

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
        /// Auto Scale service tests.
        /// </summary>
        /// <seealso cref="IAutoScaleService"/>
        public const string AutoScale = "Auto Scale";

        /// <summary>
        /// DNS service tests.
        /// </summary>
        /// <seealso cref="IDnsService"/>
        public const string Dns = "DNS";

        /// <summary>
        /// Database service tests.
        /// </summary>
        /// <seealso cref="IDatabaseService"/>
        public const string Database = "Database";

        /// <summary>
        /// Load balancer service tests.
        /// </summary>
        /// <seealso cref="ILoadBalancerProvider"/>
        /// <preliminary/>
        public const string LoadBalancers = "Load Balancers";

        /// <summary>
        /// Monitoring service tests.
        /// </summary>
        /// <seealso cref="IMonitoringProvider"/>
        /// <preliminary/>
        public const string Monitoring = "Monitoring";

        /// <summary>
        /// Queueing service tests.
        /// </summary>
        /// <seealso cref="IQueueingService"/>
        public const string Queues = "Queues";

        /// <summary>
        /// Tests Synchronous extensions to the queueing service.
        /// </summary>
        /// <seealso cref="QueueingServiceExtensions"/>
        public const string QueuesSynchronous = "Queues (Synchronous)";

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

        /// <summary>
        /// This test category should be applied to tests which should run in the automated build environment as part of
        /// validating a pull request or other change.
        /// </summary>
        public const string Unit = "Unit";
    }
}
