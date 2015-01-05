namespace OpenStackNetTests.Live
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public partial class BaseIdentityTests
    {
        private LiveTestConfiguration _configuration;

        internal TestCredentials Credentials
        {
            get
            {
                TestCredentials credentials = _configuration.TryGetSelectedCredentials();
                if (credentials == null)
                    credentials = _configuration.TryGetCredentials("TryStack_Anonymous");

                return credentials;
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
