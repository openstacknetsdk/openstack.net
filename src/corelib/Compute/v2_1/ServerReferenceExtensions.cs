using OpenStack.Compute.v2_1;
using OpenStack.Synchronous.Extensions;

// ReSharper disable once CheckNamespace
namespace OpenStack.Synchronous
{
    /// <summary />
    public static class ServerReferenceExtensions_v2_1
    {
        /// <inheritdoc cref="ServerReference.GetServerAsync"/>
        public static Server GetServer(this ServerReference server)
        {
            return server.GetServerAsync().ForceSynchronous();
        }
    }
}
