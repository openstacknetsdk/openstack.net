using OpenStack.Compute.v2_1;
using OpenStack.Synchronous.Extensions;

// ReSharper disable once CheckNamespace
namespace OpenStack.Synchronous
{
    /// <summary>
    /// Provides synchronous extention methods for a <see cref="ComputeService"/> instance.
    /// </summary>
    public static class ComputeServiceExtensions_v2_1
    {
        /// <inheritdoc cref="ComputeService.ListServersAsync" />
        public static IPage<ServerReference> ListServers(this ComputeService service, ListServersOptions options = null)
        {
            return service.ListServersAsync(options).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.GetVncConsoleAync" />
        public static Console GetVncConsole(this ComputeService service, Identifier serverId, ConsoleType type)
        {
            return service.GetVncConsoleAync(serverId, type).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.CreateKeyPairAsync" />
        public static KeyPair CreateKeyPair(this ComputeService service, string name)
        {
            return service.CreateKeyPairAsync(name).ForceSynchronous();
        }
    }
}
