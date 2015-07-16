using System;
using System.Runtime.Serialization;

namespace OpenStack
{
    /// <summary>
    /// The exception that is thrown when a a request is made for a service type that is not supported by the current provider.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    [Serializable]
    public sealed class UnsupportedServiceException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedServiceException"/> class.
        /// </summary>
        /// <param name="serviceType">The unsupported service type.</param>
        /// <param name="provider">The OpenStack provider.</param>
        public UnsupportedServiceException(ServiceType serviceType, String provider) 
            : base(string.Format("The {0} service type is not supported by the {1} provider", serviceType, provider))
        { }

        private UnsupportedServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
