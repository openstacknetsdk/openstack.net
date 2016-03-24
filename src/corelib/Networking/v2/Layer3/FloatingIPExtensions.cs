using System;
using OpenStack.Synchronous.Extensions;

namespace OpenStack.Networking.v2.Layer3.Synchronous
{
    /// <summary>
    /// Provides synchronous extention methods for a <see cref="FloatingIP"/> instance.
    /// </summary>
    public static class FloatingIPExtensions
    {
        /// <inheritdoc cref="FloatingIP.AssociateAsync" />
        /// <exception cref="InvalidOperationException">Thrown when a resource as not constructed by the SDK.</exception>
        public static void Associate(this FloatingIP floatingIP, Identifier portId)
        {
            floatingIP.AssociateAsync(portId).ForceSynchronous();
        }

        /// <inheritdoc cref="FloatingIP.DisassociateAsync" />
        /// <exception cref="InvalidOperationException">Thrown when a resource as not constructed by the SDK.</exception>
        public static void Disassociate(this FloatingIP floatingIP)
        {
            floatingIP.DisassociateAsync().ForceSynchronous();
        }
    }
}