using System;
using System.Linq;
using System.Net;
using OpenStack.Compute.v2_1.Serialization;
using OpenStack.Serialization;
using OpenStack.Synchronous;
using OpenStack.Testing;
using Xunit;

namespace OpenStack.Compute.v2_1
{
    public class VolumeTests
    {
        private readonly ComputeService _compute;

        public VolumeTests()
        {
            _compute = new ComputeService(Stubs.AuthenticationProvider, "region");
        }
        
        [Fact]
        public void GetVolume()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier volumeId = Guid.NewGuid();
                httpTest.RespondWithJson(new Volume { Id = volumeId });

                var result = _compute.GetVolume(volumeId);

                httpTest.ShouldHaveCalled($"*/os-volumes/{volumeId}");
                Assert.NotNull(result);
                Assert.Equal(volumeId, result.Id);
                Assert.IsType<ComputeApiBuilder>(((IServiceResource)result).Owner);
            }
        }

        [Fact]
        public void CreateVolume()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier volumeId = Guid.NewGuid();
                httpTest.RespondWithJson(new Volume {Id = volumeId});

                var request = new VolumeDefinition(size: 1);
                var result = _compute.CreateVolume(request);

                httpTest.ShouldHaveCalled("*/os-volumes");
                Assert.Equal(volumeId, result.Id);
                Assert.IsType<ComputeApiBuilder>(((IServiceResource)result).Owner);
            }
        }

        [Fact]
        public void ListVolumes()
        {
            using (var httpTest = new HttpTest())
            {
                Identifier volumeId = Guid.NewGuid();
                httpTest.RespondWithJson(new VolumeCollection
                {
                    new Volume {Id = volumeId}
                });

                var results = _compute.ListVolumes();

                httpTest.ShouldHaveCalled("*/os-volumes");
                Assert.Equal(1, results.Count());
                var result = results.First();
                Assert.Equal(volumeId, result.Id);
                Assert.IsType<ComputeApiBuilder>(((IServiceResource)result).Owner);
            }
        }

        [Theory]
        [InlineData(HttpStatusCode.Accepted)]
        [InlineData(HttpStatusCode.NotFound)]
        public void DeleteVolume(HttpStatusCode responseCode)
        {
            using (var httpTest = new HttpTest())
            {
                Identifier volumeId = Guid.NewGuid();
                httpTest.RespondWithJson(new Volume { Id = volumeId });
                httpTest.RespondWith((int)responseCode, "All gone!");

                var volume = _compute.GetVolume(volumeId);

                volume.Delete();
                httpTest.ShouldHaveCalled($"*/os-volumes/{volumeId}");
            }
        }
    }
}
