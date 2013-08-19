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
        /// Object storage service tests.
        /// </summary>
        public const string ObjectStorage = "ObjectStorage";

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
