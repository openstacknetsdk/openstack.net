namespace OpenStackNetTests.Live
{
    internal static class TestCategories
    {
        public const string Cleanup = "Cleanup (Simulated)";

        public const string User = "User (Simulated)";

        public const string Identity = "Identity (Simulated)";

        public const string ObjectStorage = "Object Storage (Simulated)";

        /// <summary>
        /// This test category evaluates to <c>Live</c> for live integration tests, and <c>Simulated</c> for simulated
        /// tests.
        /// </summary>
        public const string TestKind = "Simulated";

        /// <summary>
        /// This test category evaluates to <c>Live</c> for live integration tests, and <c>Unit</c> for simulated
        /// tests.
        /// </summary>
        public const string UnitTestKind = "Unit";

        /// <summary>
        /// This test category should be applied to tests which should run in the automated build environment as part of
        /// validating a pull request or other change.
        /// </summary>
        public const string Unit = "Unit";
    }
}
