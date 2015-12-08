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

        //[Fact]
        //public void GetImageExtension()
        //{
        //    using (var httpTest = new HttpTest())
        //    {
        //        Identifier imageId = Guid.NewGuid();
        //        httpTest.RespondWithJson(new ImageReferenceCollection
        //        {
        //            new ImageReference {Id = imageId}
        //        });
        //        httpTest.RespondWithJson(new Image { Id = imageId });

        //        var results = _compute.ListImages();
        //        var flavorRef = results.First();
        //        var result = flavorRef.GetImage();

        //        Assert.NotNull(result);
        //        Assert.Equal(imageId, result.Id);
        //    }
        //}

        //[Fact]
        //public void ListFlavors()
        //{
        //    using (var httpTest = new HttpTest())
        //    {
        //        const string flavorId = "1";
        //        httpTest.RespondWithJson(new FlavorReferenceCollection
        //        {
        //            Items = { new FlavorReference { Id = flavorId } }
        //        });

        //        var results = _compute.ListFlavors();

        //        httpTest.ShouldHaveCalled("*/flavors");
        //        Assert.Equal(1, results.Count());
        //        var result = results.First();
        //        Assert.Equal(flavorId, result.Id);
        //        Assert.IsType<ComputeApiBuilder>(((IServiceResource)result).Owner);
        //    }
        //}

        //[Fact]
        //public void ListFlavorDetails()
        //{
        //    using (var httpTest = new HttpTest())
        //    {
        //        const string flavorId = "1";
        //        httpTest.RespondWithJson(new FlavorCollection
        //        {
        //            Items = { new Flavor { Id = flavorId } }
        //        });
        //        httpTest.RespondWithJson(new Flavor { Id = flavorId });

        //        var results = _compute.ListFlavorDetails();

        //        httpTest.ShouldHaveCalled("*/flavors");
        //        Assert.Equal(1, results.Count());
        //        var result = results.First();
        //        Assert.Equal(flavorId, result.Id);
        //        Assert.IsType<ComputeApiBuilder>(((IServiceResource)result).Owner);
        //    }
        //}
    }
}
