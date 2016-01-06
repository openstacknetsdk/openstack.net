using OpenStack.Compute.v2_1;
using OpenStack.Synchronous.Extensions;

namespace OpenStack.Synchronous
{
    /// <summary />
    public static class ServerVolumeExtensions
    {
        /// <summary />
        public static ServerVolume GetServerVolume(this ServerVolumeReference volume)
        {
            return volume.GetServerVolumeAsync().ForceSynchronous();
        }

        /// <summary />
        public static void Detach(this ServerVolumeReference volume)
        {
            volume.DetachAsync().ForceSynchronous();
        }
    }
}