using System;
using System.Runtime.Serialization;
using System.Security;
using Newtonsoft.Json;
using RestResponse = JSIStudios.SimpleRESTServices.Client.Response;

namespace net.openstack.Core.Exceptions.Response
{
    [Serializable]
    public class ResponseException : Exception
    {
        public RestResponse Response { get; private set; }

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

        [DataContract]
        private sealed class ErrorDetails
        {
            [DataMember(Name = "code")]
            public int Code;

            [DataMember(Name = "message")]
            public string Message;

            [DataMember(Name = "details")]
            public string Details;
        }

        protected ResponseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Response = (RestResponse)info.GetValue("Response", typeof(RestResponse));
        }

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
