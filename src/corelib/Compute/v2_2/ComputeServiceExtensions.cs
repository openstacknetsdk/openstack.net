using OpenStack.Compute.v2_2;
using OpenStack.Synchronous.Extensions;

// ReSharper disable once CheckNamespace
namespace OpenStack.Synchronous
{
    /// <summary>
    /// Provides synchronous extention methods for a <see cref="ComputeService"/> instance.
    /// </summary>
    public static class ComputeServiceExtensions_v2_2
    {
        /// <inheritdoc cref="OpenStack.Compute.v2_1.ComputeService.GetVncConsoleAync" />
        public static Console GetVncConsole(this ComputeService service, Identifier serverId, ConsoleType type)
        {
            return service.GetVncConsoleAync(serverId, type).ForceSynchronous();
        }

        /// <inheritdoc cref="OpenStack.Compute.v2_1.ComputeService.CreateKeyPairAsync" />
        public static KeyPair CreateKeyPair(this ComputeService service, string name, KeyPairType? type = null)
        {
            return service.CreateKeyPairAsync(name, type).ForceSynchronous();
        }
    }
}
