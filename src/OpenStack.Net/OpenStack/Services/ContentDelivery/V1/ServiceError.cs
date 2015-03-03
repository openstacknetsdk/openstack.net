namespace OpenStack.Services.ContentDelivery.V1
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class ServiceError : ExtensibleJsonObject
    {
#pragma warning disable 649 // Field 'fieldName' is never assigned to, and will always have its default value {value}
        /// <summary>
        /// This is the backing field for the <see cref="Message"/> property.
        /// </summary>
        [JsonProperty("message", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _message;
#pragma warning restore 649

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceError"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected ServiceError()
        {
        }

        public string Message
        {
            get
            {
                return _message;
            }
        }
    }
}
