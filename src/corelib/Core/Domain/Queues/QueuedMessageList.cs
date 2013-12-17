namespace net.openstack.Core.Domain.Queues
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using net.openstack.Core.Providers;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a collection of messages stored in a queue in the <see cref="IQueueingService"/>.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonObject(MemberSerialization.OptIn)]
    public class QueuedMessageList
    {
        /// <summary>
        /// This is the backing field for the <see cref="Empty"/> property.
        /// </summary>
        private static readonly QueuedMessageList _empty = new QueuedMessageList(Enumerable.Empty<QueuedMessage>(), Enumerable.Empty<Link>());

        /// <summary>
        /// This is the backing field for the <see cref="Links"/> property.
        /// </summary>
        [JsonProperty("links")]
        private Link[] _links;

        /// <summary>
        /// This is the backing field for the <see cref="Messages"/> property.
        /// </summary>
        [JsonProperty("messages")]
        private QueuedMessage[] _messages;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueuedMessageList"/> class during
        /// JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected QueuedMessageList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueuedMessageList"/> class.
        /// </summary>
        /// <param name="messages">A collection of <see cref="QueuedMessage"/> objects describing the messages in a queue.</param>
        /// <param name="links">A collection of <see cref="Link"/> objects describing resources related to the list of messages.</param>
        /// <exception cref="ArgumentException">
        /// If <paramref name="messages"/> contains any <see langword="null"/> values.
        /// <para>-or-</para>
        /// <para>If <paramref name="links"/> contains any <see langword="null"/> values.</para>
        /// </exception>
        public QueuedMessageList(IEnumerable<QueuedMessage> messages, IEnumerable<Link> links)
        {
            if (messages != null)
            {
                _messages = messages.ToArray();
                if (_messages.Contains(null))
                    throw new ArgumentException("messages cannot contain any null values", "messages");
            }

            if (links != null)
            {
                _links = links.ToArray();
                if (_links.Contains(null))
                    throw new ArgumentException("links cannot contain any null values", "links");
            }
        }

        /// <summary>
        /// Gets an empty list of messages, which is not specific to any queue.
        /// </summary>
        public static QueuedMessageList Empty
        {
            get
            {
                return _empty;
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="Link"/> objects describing resources related
        /// to this list of messages.
        /// </summary>
        public ReadOnlyCollection<Link> Links
        {
            get
            {
                if (_links == null)
                    return null;

                return new ReadOnlyCollection<Link>(_links);
            }
        }

        /// <summary>
        /// Gets a list of <see cref="QueuedMessage"/> objects describing the messages in
        /// the queue.
        /// </summary>
        public ReadOnlyCollection<QueuedMessage> Messages
        {
            get
            {
                if (_messages == null)
                    return null;

                return new ReadOnlyCollection<QueuedMessage>(_messages);
            }
        }
    }
}
