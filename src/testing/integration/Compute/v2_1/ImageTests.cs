using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using Xunit;
using Xunit.Abstractions;

namespace OpenStack.Compute.v2_1
{
    public class ImageTests : IDisposable
    {
        private readonly ComputeService _compute;
        private readonly ComputeTestDataManager _testData;

        public ImageTests(ITestOutputHelper testLog)
        {
            var testOutput = new XunitTraceListener(testLog);
            Trace.Listeners.Add(testOutput);
            OpenStackNet.Tracing.Http.Listeners.Add(testOutput);

            var authenticationProvider = TestIdentityProvider.GetIdentityProvider();
            _compute = new ComputeService(authenticationProvider, "RegionOne");

            _testData = new ComputeTestDataManager(_compute);
        }

        public void Dispose()
        {
            Trace.Listeners.Clear();
            OpenStackNet.Tracing.Http.Listeners.Clear();

            _testData.Dispose();
        }

        [Fact]
        public async Task ListImagesTest()
        {
            var results = await _compute.ListImagesAsync(new ImageListOptions {PageSize = 1});

            while (results.Any())
            {
                var result = results.First();
                Assert.NotNull(result.Name);
                results = await results.GetNextPageAsync();
            }
            Assert.NotNull(results);
        }

        [Fact]
        public async void FindSnapshotsTest()
        {
            var server = await _testData.CreateServer();
            await server.WaitUntilActiveAsync();
            var snapshot = await server.SnapshotAsync(new SnapshotServerRequest(server.Name + "SNAPSHOT"));
            _testData.Register(snapshot);

            var results = await _compute.ListImageDetailsAsync(new ImageListOptions {Type = ImageType.Snapshot});
            Assert.NotNull(results);
            Assert.All(results, x => Assert.Equal(ImageType.Snapshot, x.Type));
            Assert.Contains(results, image => image.Id == snapshot.Id);
        }

        [Fact]
        public async Task ListImageDetailsTest()
        {
            var results = await _compute.ListImageDetailsAsync(new ImageListOptions { PageSize = 1 });

            while (results.Any())
            {
                var result = results.First();
                Assert.NotNull(result.Size);
                results = await results.GetNextPageAsync();
            }
            Assert.NotNull(results);
        }

        [Fact]
        public async void SetImageMetadataTest()
        {
            var server = await _testData.CreateServer();
            await server.WaitUntilActiveAsync();
            var snapshot = await server.SnapshotAsync(new SnapshotServerRequest(server.Name + "SNAPSHOT")
            {
                Metadata = {["category"] = "ci-test"}
            });
            _testData.Register(snapshot);

            var metadata = await _compute.GetImageMetadataAsync(snapshot.Id);
            Assert.NotNull(metadata);
            Assert.True(metadata.ContainsKey("category"));
        }

        //[Fact]
        //public async void DeleteImageTest()
        //{
        //    Image image = await _testData.CreateImage();
        //    Trace.WriteLine($"Created image named: {image.Name}");

        //    await image.DeleteAsync();
        //    await image.WaitUntilDeletedAsync();

        //    await Assert.ThrowsAsync<FlurlHttpException>(() => _compute.GetImageAsync(image.Id));
        //}
    }
}
