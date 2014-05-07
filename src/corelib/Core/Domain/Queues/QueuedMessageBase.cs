using System;
using System.Linq;

namespace net.openstack.Core.Domain.Queues
{
    /// <summary>
    /// Contains common methods used across queue message domain objects.
    /// </summary>
    public abstract class QueuedMessageBase
    {
        /// <summary>
        /// Parses a URI to extract a <see cref="MessageId"/>.
        /// </summary>
        /// <param name="href">The resource URI.</param>
        /// <returns>The ID of the message.</returns>
        protected MessageId ParseMessageId(Uri href)
        {
            // make sure we have an absolute URI, or Segments will throw an InvalidOperationException
            if (!href.IsAbsoluteUri)
                href = new Uri(new Uri("http://example.com"), href);

            if (href.Segments.Length == 0)
                return null;

            return new MessageId(href.Segments.Last());
        }
    }
}