using System;
using System.Runtime.Serialization;
using System.Security;
using Newtonsoft.Json;
using RestResponse = JSIStudios.SimpleRESTServices.Client.Response;

namespace net.openstack.Core.Exceptions.Response
{
    /// <summary>
    /// Represents errors resulting from a call to a REST API.
    /// </summary>
    [Serializable]
    public abstract class ResponseException : Exception
    {
        /// <summary>
        /// Gets the REST <see cref="RestResponse"/> containing the details
        /// about this error.
        /// </summary>
        public RestResponse Response
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseException"/> class with the
        /// specified error message and REST response.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="response">The REST response.</param>
        public ResponseException(string message, RestResponse response)
            : base(message)
        {
            Response = response;
        }

        /// <inheritdoc/>
        public override string Message
        {
            get
            {
                if (Response != null && Response.HasJsonBody())
                {
                    try
                    {
                        ErrorResponse response = JsonConvert.DeserializeObject<ErrorResponse>(Response.RawBody);
                        if (!string.IsNullOrEmpty(response.Message))
                            return response.Message;
                    }
                    catch (JsonSerializationException)
                    {
                        // will fall back to base.Message
                    }
                }

                return base.Message;
            }
        }

        [Serializable]
        private sealed class ErrorResponse : ISerializable
        {
            private readonly string _errorKind;
            private readonly ErrorDetails _errorDetails;

            private ErrorResponse(SerializationInfo info, StreamingContext context)
            {
                if (info.MemberCount != 1)
                    throw new ArgumentException();

                foreach (SerializationEntry entry in info)
                {
                    _errorKind = entry.Name;
                    _errorDetails = JsonConvert.DeserializeObject<ErrorDetails>(entry.Value.ToString());
                }
            }

            public string ErrorKind
            {
                get
                {
                    return _errorKind;
                }
            }

            public int Code
            {
                get
                {
                    return _errorDetails.Code;
                }
            }

            public string Message
            {
                get
                {
                    return _errorDetails.Message;
                }
            }

            public string Details
            {
                get
                {
                    return _errorDetails.Details;
                }
            }

            [SecurityCritical]
            void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
            {
                throw new NotImplementedException();
            }
        }

        [JsonObject(MemberSerialization.OptIn)]
        private sealed class ErrorDetails
        {
#pragma warning disable 649 // Field '{fieldname}' is never assigned to, and will always have its default value {0 | null}
            [JsonProperty("code")]
            public int Code;

            [JsonProperty("message")]
            public string Message;

            [JsonProperty("details")]
            public string Details;
#pragma warning restore 649
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseException"/> class with
        /// serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="info"/> is <c>null</c>.</exception>
        protected ResponseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            Response = (RestResponse)info.GetValue("Response", typeof(RestResponse));
        }

        /// <inheritdoc/>
        [SecurityCritical]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            base.GetObjectData(info, context);
            info.AddValue("Response", Response);
        }
    }
}
