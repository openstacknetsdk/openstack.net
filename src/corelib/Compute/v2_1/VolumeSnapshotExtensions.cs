using OpenStack.Compute.v2_1;
using OpenStack.Synchronous.Extensions;


// ReSharper disable once CheckNamespace
namespace OpenStack.Synchronous
{
    /// <summary />
    public static class VolumeSnapshotExtensions_v2_1
    {
        /// <inheritdoc cref="VolumeSnapshot.DeleteAsync"/>
        public static void Delete(this VolumeSnapshot volume)
        {
            volume.DeleteAsync().ForceSynchronous();
        }
    }
}
