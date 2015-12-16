using OpenStack.Compute.v2_1;
using OpenStack.Synchronous.Extensions;

namespace OpenStack.Synchronous
{
    /// <summary />
    public static class VolumeAttachmentExtensions
    {
        /// <summary />
        public static VolumeAttachment GetServerVolume(this VolumeReference volume)
        {
            return volume.GetServerVolumeAsync().ForceSynchronous();
        }

        /// <summary />
        public static void Detach(this VolumeReference volume)
        {
            volume.DetachAsync().ForceSynchronous();
        }
    }
}