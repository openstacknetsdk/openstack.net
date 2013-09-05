namespace net.openstack.Core.Domain.Converters
{
    using System;
    using System.Globalization;
    using System.Net;
    using Newtonsoft.Json;

    public class IPAddressSimpleConverter : JsonConverter
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

            serializer.Serialize(writer, address.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType != typeof(IPAddress))
                throw new JsonSerializationException(string.Format(CultureInfo.InvariantCulture, "Expected target type {0}, found {1}.", typeof(IPAddress), objectType));

            string address = serializer.Deserialize<string>(reader);
            if (address == null)
                return null;

            return IPAddress.Parse(address);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IPAddress);
        }
    }
}
