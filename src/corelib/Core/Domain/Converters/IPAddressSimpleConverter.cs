namespace net.openstack.Core.Domain.Converters
{
    using System;
    using System.Globalization;
    using System.Net;
    using Newtonsoft.Json;

    /// <summary>
    /// This implementation of <see cref="JsonConverter"/> allows for JSON serialization
    /// and deserialization of <see cref="IPAddress"/> objects using a simple string
    /// representation. Serialization is performed using <see cref="IPAddress.ToString"/>,
    /// and deserialization is performed using <see cref="IPAddress.Parse"/>.
    /// </summary>
    /// <seealso href="http://docs.openstack.org/api/openstack-compute/2/content/List_Addresses-d1e3014.html">List Addresses (OpenStack Compute API v2 and Extensions Reference)</seealso>
    /// <seealso href="http://docs.openstack.org/api/openstack-compute/2/content/List_Addresses_by_Network-d1e3118.html">List Addresses by Network (OpenStack Compute API v2 and Extensions Reference)</seealso>
    public class IPAddressSimpleConverter : JsonConverter
    {
        /// <remarks>
        /// Serialization is performed by calling <see cref="IPAddress.ToString"/> and writing
        /// the result as a string value.
        /// </remarks>
        /// <inheritdoc/>
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

            serializer.Serialize(writer, address.ToString());
        }

        /// <remarks>
        /// Deserialization is performed by reading the raw value as a string and using
        /// <see cref="IPAddress.Parse"/> to convert it to an <see cref="IPAddress"/>.
        /// </remarks>
        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType != typeof(IPAddress))
                throw new JsonSerializationException(string.Format(CultureInfo.InvariantCulture, "Expected target type {0}, found {1}.", typeof(IPAddress), objectType));

            string address = serializer.Deserialize<string>(reader);
            if (address == null)
                return null;

            return IPAddress.Parse(address);
        }

        /// <returns><c>true</c> if <paramref name="objectType"/> equals <see cref="IPAddress"/>; otherwise, <c>false</c>.</returns>
        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IPAddress);
        }
    }
}
