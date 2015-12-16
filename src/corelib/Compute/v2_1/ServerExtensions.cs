using System;
using System.Collections.Generic;
using OpenStack.Compute.v2_1;
using OpenStack.Synchronous.Extensions;

// ReSharper disable once CheckNamespace
namespace OpenStack.Synchronous
{
    /// <summary />
    public static class ServerExtensions_v2_1
    {
        /// <inheritdoc cref="ServerReference.GetServerAsync"/>
        public static Server GetServer(this ServerReference server)
        {
            return server.GetServerAsync().ForceSynchronous();
        }

        /// <inheritdoc cref="ServerReference.GetServerAsync"/>
        public static IList<ServerAddress> GetAddress(this ServerReference server, string key)
        {
            return server.GetAddressAsync(key).ForceSynchronous();
        }

        /// <inheritdoc cref="ServerReference.GetServerAsync"/>
        public static IDictionary<string, IList<ServerAddress>> ListAddresses(this ServerReference server)
        {
            return server.ListAddressesAsync().ForceSynchronous();
        }

        /// <inheritdoc cref="Server.WaitUntilActiveAsync"/>
        public static void WaitUntilActive(this Server server, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null)
        {
            server.WaitUntilActiveAsync(refreshDelay, timeout, progress).ForceSynchronous();
        }

        /// <inheritdoc cref="Server.WaitUntilDeletedAsync"/>
        public static void WaitUntilDeleted(this Server server, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null)
        {
            server.WaitUntilDeletedAsync(refreshDelay, timeout, progress).ForceSynchronous();
        }

        /// <inheritdoc cref="Server.UpdateAsync"/>
        public static void Update(this Server server)
        {
            server.UpdateAsync().ForceSynchronous();
        }

        /// <inheritdoc cref="ServerReference.DeleteAsync"/>
        public static void Delete(this Server server)
        {
            server.DeleteAsync().ForceSynchronous();
        }

        /// <inheritdoc cref="ServerReference.SnapshotAsync"/>
        public static Image Snapshot(this ServerReference server, SnapshotServerRequest request)
        {
            return server.SnapshotAsync(request).ForceSynchronous();
        }

        /// <inheritdoc cref="ServerReference.StartAsync"/>
        public static void Start(this ServerReference server)
        {
            server.StartAsync().ForceSynchronous();
        }

        /// <inheritdoc cref="ServerReference.StopAsync"/>
        public static void Stop(this ServerReference server)
        {
            server.StopAsync().ForceSynchronous();
        }

        /// <inheritdoc cref="ServerReference.RebootAsync"/>
        public static void Reboot(this ServerReference server, RebootServerRequest request = null)
        {
            server.RebootAsync(request).ForceSynchronous();
        }

        /// <inheritdoc cref="ServerReference.EvacuateAsync"/>
        public static void Evacuate(this ServerReference server, EvacuateServerRequest request)
        {
            server.EvacuateAsync(request).ForceSynchronous();
        }

        /// <inheritdoc cref="ServerReference.AttachVolumeAsync"/>
        public static ServerVolume AttachVolume(this ServerReference server, ServerVolumeDefinition volume)
        {
            return server.AttachVolumeAsync(volume).ForceSynchronous();
        }

        /// <inheritdoc cref="ServerReference.ListVolumesAsync"/>
        public static IEnumerable<ServerVolume> ListVolumes(this ServerReference server)
        {
            return server.ListVolumesAsync().ForceSynchronous();
        }

        /// <inheritdoc cref="ServerReference.GetVncConsoleAsync"/>
        public static RemoteConsole GetVncConsole(this ServerReference server, RemoteConsoleType type)
        {
            return server.GetVncConsoleAsync(type).ForceSynchronous();
        }

        /// <inheritdoc cref="ServerReference.GetSpiceConsoleAsync"/>
        public static RemoteConsole GetSpiceConsole(this ServerReference server)
        {
            return server.GetSpiceConsoleAsync().ForceSynchronous();
        }

        /// <inheritdoc cref="ServerReference.GetSpiceConsoleAsync"/>
        public static RemoteConsole GetSerialConsole(this ServerReference server)
        {
            return server.GetSerialConsoleAsync().ForceSynchronous();
        }

        /// <inheritdoc cref="ServerReference.GetRdpConsoleAsync"/>
        public static RemoteConsole GetRdpConsole(this ServerReference server)
        {
            return server.GetRdpConsoleAsync().ForceSynchronous();
        }

        /// <inheritdoc cref="ServerReference.GetConsoleOutputAsync"/>
        public static string GetConsoleOutput(this ServerReference server, int length = -1)
        {
            return server.GetConsoleOutputAsync(length).ForceSynchronous();
        }

        /// <inheritdoc cref="ServerReference.RescueAsync"/>
        public static string Rescue(this ServerReference server, RescueServerRequest request = null)
        {
            return server.RescueAsync(request).ForceSynchronous();
        }

        /// <inheritdoc cref="ServerReference.RescueAsync"/>
        public static void Unrescue(this ServerReference server)
        {
            server.UnrescueAsync().ForceSynchronous();
        }
    }
}
