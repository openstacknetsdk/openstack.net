using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenStack.Images.v2;
using Xunit;
using Flurl.Http.Testing;
using OpenStack.Compute.v2_1.Serialization;
using OpenStack.Images.v2.Serialization;
using OpenStack.Images.v2.Synchronous;

namespace OpenStack.Image.v2
{
    public class ImageTests
    {
        private readonly ImageService _imageService;

        public ImageTests()
        {
            _imageService = new ImageService(Stubs.AuthenticationProvider, "region");
        }

        [Fact]
        public void ListNetworks()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier imageId = Guid.NewGuid();
                ImageOptions image = new ImageOptions { Id = imageId, Status = ImageStatus.Active };
                httpTest.RespondWithJson(new ImageOptionsCollection { image});

                var images = _imageService.ListImages();

                httpTest.ShouldHaveCalled("*/images");
                Assert.NotNull(images);
                Assert.Equal(1, images.Count());
                Assert.Equal(imageId, images.First().Id);
                Assert.Equal(ImageStatus.Active, images.First().Status);
            }
        }
    }
}
