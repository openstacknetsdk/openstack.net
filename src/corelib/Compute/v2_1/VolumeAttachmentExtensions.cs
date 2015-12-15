using OpenStack.Compute.v2_1;
using OpenStack.Synchronous.Extensions;

namespace OpenStack.Synchronous
{
    /// <summary />
    public static class VolumeAttachmentExtensions
    {
        /// <summary />
        public static void Detach(this VolumeReference volume)
        {
            volume.DetachAsync().ForceSynchronous();
        }
    }
}