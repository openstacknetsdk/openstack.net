using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using OpenStack.Networking.v2;

namespace OpenStack.ObjectStorage.v1.Serialization
{
    /// <summary>
    /// Handles serialization/deserialization for <see cref="System.Net.HttpStatusCode"/>.
    /// </summary>
    public class HttpStatusCodeConverter : JsonConverter
    {
        /// <summary>
        /// Writes the json.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The serializer.</param>
        /// <exception cref="NotImplementedException"></exception>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                return;

			var httpStatus = (System.Net.HttpStatusCode)value;

            writer.WriteValue(string.Format("{0} {1}", (int)httpStatus, httpStatus.ToString()));
        }

        /// <summary>
        /// Reads the json.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value.</param>
        /// <param name="serializer">The serializer.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String) throw new JsonSerializationException("Unexpected end when reading HttpStatusCode.");

	        var strValue = reader.Value.ToString();

	        return (System.Net.HttpStatusCode) int.Parse(strValue.Substring(0, strValue.IndexOf(" ")));
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof (System.Net.HttpStatusCode).IsAssignableFrom(objectType);
        }
    }
}