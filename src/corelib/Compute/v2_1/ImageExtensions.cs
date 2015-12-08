using OpenStack.Compute.v2_1;
using OpenStack.Synchronous.Extensions;

// ReSharper disable once CheckNamespace
namespace OpenStack.Synchronous
{
    /// <summary />
    public static class ImageExtensions_v2_1
    {
        /// <inheritdoc cref="ImageReference.GetImageAsync"/>
        public static Image GetImage(this ImageReference image)
        {
            return image.GetImageAsync().ForceSynchronous();
        }
    }
}
