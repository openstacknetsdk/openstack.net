using System;
using OpenStack.Compute.v2_1;
using OpenStack.Synchronous.Extensions;
using Console = OpenStack.Compute.v2_1.Console;

// ReSharper disable once CheckNamespace
namespace OpenStack.Synchronous
{
    /// <summary>
    /// Provides synchronous extention methods for a <see cref="ComputeService"/> instance.
    /// </summary>
    public static class ComputeServiceExtensions_v2_1
    {
        /// <inheritdoc cref="ComputeService.GetServerAsync" />
        public static Server GetServer(this ComputeService service, Identifier serverId)
        {
            return service.GetServerAsync(serverId).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.CreateServerAsync" />
        public static Server CreateServer(this ComputeService service, ServerCreateDefinition server)
        {
            return service.CreateServerAsync(server).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.WaitUntilServerIsActiveAsync" />
        public static Server WaitUntilServerIsActive(this ComputeService service, Identifier serverId, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null)
        {
            return service.WaitUntilServerIsActiveAsync(serverId, refreshDelay, timeout, progress).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.ListServersAsync" />
        public static IPage<ServerReference> ListServers(this ComputeService service, ListServersOptions options = null)
        {
            return service.ListServersAsync(options).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.ListServerDetailsAsync" />
        public static IPage<Server> ListServerDetails(this ComputeService service, ListServersOptions options = null)
        {
            return service.ListServerDetailsAsync(options).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.UpdateServerAsync" />
        public static Server UpdateServer(this ComputeService service, Identifier serverid, ServerUpdateDefinition server)
        {
            return service.UpdateServerAsync(serverid, server).ForceSynchronous();
        }

        /// <inheritdoc cref="ComputeService.DeleteServerAsync" />
        public static void DeleteServer(this ComputeService service, Identifier serverId)
        {
            service.DeleteServerAsync(serverId).ForceSynchronous();
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
