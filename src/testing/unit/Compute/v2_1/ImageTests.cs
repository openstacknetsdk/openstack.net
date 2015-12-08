using System;
using System.Linq;
using OpenStack.Compute.v2_1.Serialization;
using OpenStack.Serialization;
using OpenStack.Synchronous;
using OpenStack.Testing;
using Xunit;

namespace OpenStack.Compute.v2_1
{
    public class ImageTests
    {
        private readonly ComputeService _compute;

        public ImageTests()
        {
            _compute = new ComputeService(Stubs.AuthenticationProvider, "region");
        }
        
        [Fact]
        public void GetImage()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier imageId = "1";
                httpTest.RespondWithJson(new Image { Id = imageId });

                var result = _compute.GetImage(imageId);

                httpTest.ShouldHaveCalled($"*/images/{imageId}");
                Assert.NotNull(result);
                Assert.Equal(imageId, result.Id);
                Assert.IsType<ComputeApiBuilder>(((IServiceResource)result).Owner);
            }
        }

        [Fact]
        public void GetImageExtension()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier imageId = Guid.NewGuid();
                httpTest.RespondWithJson(new ImageReferenceCollection
                {
                    new ImageReference {Id = imageId}
                });
                httpTest.RespondWithJson(new Image { Id = imageId });

                var results = _compute.ListImages();
                var flavorRef = results.First();
                var result = flavorRef.GetImage();

                Assert.NotNull(result);
                Assert.Equal(imageId, result.Id);
            }
        }

        [Fact]
        public void ListImages()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier imageId = Guid.NewGuid();
                httpTest.RespondWithJson(new ImageReferenceCollection
                {
                    Items = { new ImageReference { Id = imageId } },
                    Links = { new PageLink("next", "http://api.com/next") }
                });

                var results = _compute.ListImages();

                httpTest.ShouldHaveCalled("*/images");
                Assert.Equal(1, results.Count());
                var result = results.First();
                Assert.Equal(imageId, result.Id);
                Assert.IsType<ComputeApiBuilder>(((IServiceResource)result).Owner);
            }
        }

        [Fact]
        public void ListImagesWithFilter()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWithJson(new ImageCollection());

                const string name = "foo";
                const int minRam = 2;
                const int minDisk = 1;
                Identifier serverId = Guid.NewGuid();
                var lastModified = DateTimeOffset.Now.AddDays(-1);
                var imageType = ImageType.Snapshot;
                
                _compute.ListImages(new ImageListOptions { Name = name, ServerId = serverId, LastModified = lastModified, MininumDiskSize = minDisk, MininumMemorySize = minRam, Type = imageType});

                httpTest.ShouldHaveCalled($"*name={name}");
                httpTest.ShouldHaveCalled($"*server={serverId}");
                httpTest.ShouldHaveCalled($"*minRam={minRam}");
                httpTest.ShouldHaveCalled($"*minDisk={minDisk}");
                httpTest.ShouldHaveCalled($"*type={imageType}");
                httpTest.ShouldHaveCalled("*changes-since=");
            }
        }

        [Fact]
        public void ListImagesWithPaging()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWithJson(new ImageCollection());

                Identifier startingAt = Guid.NewGuid();
                const int pageSize = 10;
                _compute.ListImages(new ImageListOptions { PageSize = pageSize, StartingAt = startingAt });

                httpTest.ShouldHaveCalled($"*marker={startingAt}*");
                httpTest.ShouldHaveCalled($"*limit={pageSize}*");
            }
        }
    }
}
