using System;
using System.Collections.Generic;
using net.openstack.Core.Providers;
using Newtonsoft.Json;

namespace net.openstack.Core.Domain.Queues
{
    /// <summary>
    /// Represents the response returned when enqueueing messages in the <see cref="IQueueingService"/>
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class MessagesEnqueued : QueuedMessageBase
    {
#pragma warning disable 649 // Field 'fieldName' is never assigned to, and will always have its default value {value}
        /// <summary>
        /// Specifies whether the enqueue was only partially successful. 
        /// Successfully enqueued messages are listed under <see cref="_resources"/>
        /// </summary>
        [JsonProperty("partial")]
        private bool _partial;

        /// <summary>
        /// Contains a collection of message resource URIs.
        /// </summary>
        [JsonProperty("resources")]
        private IEnumerable<Uri> _resources;
#pragma warning restore 649

        /// <summary>
        /// Initializes a new instance of <see cref="MessagesEnqueued"/>
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected MessagesEnqueued()
        {
        }

        /// <summary>
        /// Gets the posted message IDs.
        /// </summary>
        public IEnumerable<MessageId> Ids
        {
            get
            {
                if (_resources == null) 
                    yield break;

                foreach (var resource in _resources)
                {
                    yield return ParseMessageId(resource);
                }
            }
        }
    }
}