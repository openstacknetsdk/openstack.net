using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace OpenStack.ContentDeliveryNetworks.v1
{
    /// <summary>
    /// The exception that is thrown when a <see cref="IContentDeliveryNetworkService"/> service operation (Create, Delete, Update) fails.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    [Serializable]
    public class ServiceOperationFailedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceOperationFailedException"/> class.
        /// </summary>
        public ServiceOperationFailedException(IEnumerable<ServiceError> errors)
        {
            Errors = errors;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceOperationFailedException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected ServiceOperationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Errors = JsonConvert.DeserializeObject<IEnumerable<ServiceError>>(info.GetString("service_errors"));
        }

        /// <summary>
        /// Errors generated during the previous service operation.
        /// </summary>
        public IEnumerable<ServiceError> Errors { get; private set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("service_errors", JsonConvert.SerializeObject(Errors));
            base.GetObjectData(info, context);
        }
    }
}
