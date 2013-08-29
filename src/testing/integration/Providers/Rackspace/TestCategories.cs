namespace Net.OpenStack.Testing.Integration.Providers.Rackspace
{
    /// <summary>
    /// Provides predefined categories for use with the <see cref="TestCategoryAttribute"/>.
    /// </summary>
    public static class TestCategories
    {
        /// <summary>
        /// Identity service tests.
        /// </summary>
        public const string Identity = "Identity";

        /// <summary>
        /// Block storage service tests.
        /// </summary>
        public const string BlockStorage = "BlockStorage";

        /// <summary>
        /// Object storage service tests.
        /// </summary>
        public const string ObjectStorage = "ObjectStorage";

        /// <summary>
        /// Networks service tests.
        /// </summary>
        public const string Networks = "Networks";

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
