using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using net.openstack.Core.Exceptions.Response;
using OpenStack;
using Xunit;
using Xunit.Abstractions;

namespace net.openstack.Providers.Rackspace
{
    public class CloudFilesTests
    {
        private const string region = "RegionOne";
        private readonly CloudFilesProvider _cloudFiles;

        public CloudFilesTests(ITestOutputHelper testLog)
        {
            var testOutput = new XunitTraceListener(testLog);
            Trace.Listeners.Add(testOutput);
            OpenStackNet.Tracing.Http.Listeners.Add(testOutput);

            var authenticationProvider = TestIdentityProvider.GetIdentityProvider();
            authenticationProvider.Authenticate();

            _cloudFiles = new CloudFilesProvider(null, authenticationProvider);
        }

        public void Dispose()
        {
            Trace.Listeners.Clear();
            OpenStackNet.Tracing.Http.Listeners.Clear();
        }

        [Fact]
        public void MoveObject()
        {
            const string container = "test";
            const string fileName = "helloworld.html";
            const string backupName = "helloworld.bak";

            // Remove old test data
            try
            {
                _cloudFiles.DeleteObject(container, backupName, region: region);
            }
            catch (ItemNotFoundException)
            {
            }

            // Create test data
            _cloudFiles.CreateContainer(container, region: region);
            var testObject = new MemoryStream(Encoding.UTF8.GetBytes("hello world"));
            _cloudFiles.CreateObject(container, testObject, fileName, region: region);

            // Backup the file
            _cloudFiles.MoveObject(container, fileName, container, backupName, region: region);

            // Verify the file moved
            var files = _cloudFiles.ListObjects(container, region: region);
            Assert.False(files.Any(f => f.Name == fileName));
            Assert.True(files.Any(f => f.Name == backupName));
        }

        [Fact]
        public void MoveObject_ShouldDoNothing_WhenSourceAndDestinationAreTheSame()
        {
            const string container = "test";
            const string filename = "helloworld.html";

            // Create test data
            _cloudFiles.CreateContainer(container, region: region);
            var testObject = new MemoryStream(Encoding.UTF8.GetBytes("hello world"));
            _cloudFiles.CreateObject(container, testObject, filename, region: region);

            // Move the file to the same location
            _cloudFiles.MoveObject(container, filename, container, filename, region: region);

            // Verify the file wasn't removed
            var files = _cloudFiles.ListObjects(container, region: region);
            Assert.True(files.Any(f => f.Name == filename));
        }
    }
}
