namespace OpenStack.Net
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Runtime.Serialization;

    /// <summary>
    /// The exception that is thrown when an <see cref="HttpResponseMessage"/> indicates
    /// a <see cref="F:System.Net.WebExceptionStatus.ProtocolError"/> occurred during an HTTP request
    /// sent using <see cref="HttpClient"/>.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [Serializable]
    public class HttpWebException : WebException
    {
#if !PORTABLE
        private const WebExceptionStatus ProtocolError = WebExceptionStatus.ProtocolError;
#else
        private const WebExceptionStatus ProtocolError = WebExceptionStatus.UnknownError;
#endif

        [NonSerialized]
        private ExceptionData _state;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpWebException"/> class
        /// with the specified response message.
        /// </summary>
        /// <remarks>
        /// This method does not specify the reason why the HTTP response indicates a failure.
        /// In most cases, the failure is either due to the HTTP status code indicating an
        /// error (<see cref="HttpResponseMessage.IsSuccessStatusCode"/> is <see langword="false"/>),
        /// or the response headers or body indicates a failure occurred with respect to a
        /// particular API call.
        /// </remarks>
        /// <param name="response">The response to the web request.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="response"/> is <see langword="null"/>.</exception>
        public HttpWebException(HttpResponseMessage response)
            : base(response.ReasonPhrase, ProtocolError)
        {
            if (response == null)
                throw new ArgumentNullException("response");

            _state.ResponseMessage = response;
#if NET40PLUS && !PORTABLE
            SerializeObjectState += (ex, args) => args.AddSerializedState(_state);
#endif
        }

        /// <summary>
        /// Gets the <see cref="HttpResponseMessage"/> for the web request.
        /// </summary>
        public HttpResponseMessage ResponseMessage
        {
            get
            {
                return _state.ResponseMessage;
            }
        }

        [Serializable]
        private struct ExceptionData : ISafeSerializationData
        {
            public HttpResponseMessage ResponseMessage
            {
                get;
                set;
            }

            void ISafeSerializationData.CompleteDeserialization(object deserialized)
            {
                ((HttpWebException)deserialized)._state = this;
            }
        }
    }
}
