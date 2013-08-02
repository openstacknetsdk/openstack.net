using System;
using System.Net;
using System.Runtime.Serialization;
using RestResponse = JSIStudios.SimpleRESTServices.Client.Response;

namespace net.openstack.Core.Exceptions.Response
{
    /// <summary>
    /// Represents errors with status code <see cref="HttpStatusCode.ServiceUnavailable"/> resulting
    /// from a call to a REST API.
    /// </summary>
    [Serializable]
    public class ServiceUnavailableException : ResponseException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceUnavailableException"/> class with the
        /// specified REST response.
        /// </summary>
        /// <param name="response">The REST response.</param>
        public ServiceUnavailableException(RestResponse response)
            : base("The service is currently unavailable. Please try again later.", response)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceUnavailableException"/> class with the
        /// specified error message and REST response.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="response">The REST response.</param>
        public ServiceUnavailableException(string message, RestResponse response)
            : base(message, response)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceUnavailableException"/> class with
        /// serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="info"/> is <c>null</c>.</exception>
        protected ServiceUnavailableException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
