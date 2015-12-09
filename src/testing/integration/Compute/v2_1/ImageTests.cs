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
            var results = await _compute.ListImageDetailsAsync(new ImageListOptions {Type = ImageType.Snapshot});
            Assert.NotNull(results);
            Assert.All(results, image => Assert.Equal(ImageType.Snapshot, image.Type));
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
