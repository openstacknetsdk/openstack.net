namespace OpenStackNetTests.Live
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public partial class ObjectStorageTests
    {
        private LiveTestConfiguration _configuration;

        internal TestCredentials Credentials
        {
            get
            {
                if (_configuration == null)
                    return null;

                return _configuration.TryGetSelectedCredentials();
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _configuration = LiveTestConfiguration.LoadDefaultConfiguration();
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }
    }
}
