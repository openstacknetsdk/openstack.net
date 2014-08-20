namespace OpenStack.Net
{
    using System;
    using System.Net.Http;

    /// <summary>
    /// This class contains data for events that happen in the context of an
    /// HTTP request.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class HttpRequestEventArgs : EventArgs
    {
        /// <summary>
        /// This is the backing field for the <see cref="Request"/> property.
        /// </summary>
        private readonly HttpRequestMessage _request;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestEventArgs"/> class
        /// with the specified web request.
        /// </summary>
        /// <param name="request">The HTTP web request.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="request"/> is <see langword="null"/>.</exception>
        public HttpRequestEventArgs(HttpRequestMessage request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            _request = request;
        }

        /// <summary>
        /// Gets the <see cref="HttpRequestMessage"/> associated with the event.
        /// </summary>
        public HttpRequestMessage Request
        {
            get
            {
                return _request;
            }
        }
    }
}
