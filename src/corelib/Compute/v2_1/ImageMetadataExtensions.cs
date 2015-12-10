using OpenStack.Compute.v2_1;
using OpenStack.Synchronous.Extensions;

// ReSharper disable once CheckNamespace
namespace OpenStack.Synchronous
{
    /// <summary />
    public static class ImageMetadataExtensions_v2_1
    {
        /// <inheritdoc cref="ImageReference.GetImageAsync"/>
        public static void Update(this ImageMetadata metadata, bool overwrite = false)
        {
            metadata.UpdateAsync(overwrite).ForceSynchronous();
        }
    }
}
