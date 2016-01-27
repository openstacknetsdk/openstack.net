using OpenStack.Compute.v2_1;
using OpenStack.Synchronous.Extensions;


// ReSharper disable once CheckNamespace
namespace OpenStack.Synchronous
{
    /// <summary />
    public static class VolumeExtensions_v2_1
    {
        /// <inheritdoc cref="Volume.DeleteAsync"/>
        public static void Delete(this Volume volume)
        {
            volume.DeleteAsync().ForceSynchronous();
        }
    }
}
