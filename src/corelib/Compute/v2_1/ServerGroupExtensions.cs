using OpenStack.Compute.v2_1;
using OpenStack.Synchronous.Extensions;


// ReSharper disable once CheckNamespace
namespace OpenStack.Synchronous
{
    /// <summary />
    public static class ServerGroupExtensions_v2_1
    {
        /// <inheritdoc cref="ServerGroup.DeleteAsync"/>
        public static void Delete(this ServerGroup group)
        {
            group.DeleteAsync().ForceSynchronous();
        }
    }
}
