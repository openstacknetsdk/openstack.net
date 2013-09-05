namespace net.openstack.Core.Domain.Converters
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Net.Sockets;
    using net.openstack.Core.Providers;
    using Newtonsoft.Json;

    public class IPAddressDetailsConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            IPAddress address = value as IPAddress;
            if (address == null)
                throw new JsonSerializationException(string.Format(CultureInfo.InvariantCulture, "Unexpected value when converting IP address. Expected {0}, got {1}.", typeof(IPAddress), value.GetType()));

            AddressDetails details = new AddressDetails(address);
            serializer.Serialize(writer, details);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType != typeof(IPAddress))
                throw new JsonSerializationException(string.Format(CultureInfo.InvariantCulture, "Expected target type {0}, found {1}.", typeof(IPAddress), objectType));

            AddressDetails details = serializer.Deserialize<AddressDetails>(reader);
            if (details == null)
                return null;

            return IPAddress.Parse(details.Address);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IPAddress);
        }

        /// <summary>
        /// Represents a network address in a format compatible with communication with the Compute Service APIs.
        /// </summary>
        /// <seealso cref="IComputeProvider.ListAddresses"/>
        /// <seealso cref="IComputeProvider.ListAddressesByNetwork"/>
        [JsonObject(MemberSerialization.OptIn)]
        private class AddressDetails
        {
            /// <summary>
            /// Gets the network address. This is an IPv4 address if <see cref="Version"/> is <c>"4"</c>,
            /// or an IPv6 address if <see cref="Version"/> is <c>"6"</c>.
            /// </summary>
            [JsonProperty("addr")]
            public string Address
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets the network address version. The value is either <c>"4"</c> or <c>"6"</c>.
            /// </summary>
            [JsonProperty("version")]
            public string Version
            {
                get;
                private set;
            }

            public AddressDetails()
            {
            }

            public AddressDetails(IPAddress address)
            {
                Address = address.ToString();
                switch (address.AddressFamily)
                {
                case AddressFamily.InterNetwork:
                    Version = "4";
                    break;

                case AddressFamily.InterNetworkV6:
                    Version = "6";
                    break;

                default:
                    throw new ArgumentException("The specified address family is not supported.");
                }
            }
        }
    }
}
