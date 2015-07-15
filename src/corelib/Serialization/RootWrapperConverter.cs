using System;
using Newtonsoft.Json;

namespace OpenStack.Serialization
{
    /// <summary>
    /// Some of the OpenStack API's like to return a wrapper around the real object requested. This will deal with that root level wrapper.
    /// <para>Note that it only affects the root, if there are nested objects that also use this converter, it is assumed that they don't have a wrapper.</para>
    /// </summary>
    internal class RootWrapperConverter : JsonConverter
    {
        private readonly string _name;

        /// These are on by default so that the converter is picked up and used. 
        /// Once we have wrapped/unwrapped, we set to false so tha the default serialization/deserialization logic is used.
        private static bool _canRead = true;
        private static bool _canWrite = true;
        private static readonly object ReadLock = new object();
        private static readonly object WriteLock = new object();

        public RootWrapperConverter(string name)
        {
            _name = name;
        }

        public override bool CanRead
        {
            get
            {
                lock (ReadLock)
                {
                    return _canRead;
                }
            }
        }

        public override bool CanWrite
        {
            get
            {
                lock (WriteLock)
                {
                    return _canWrite;
                }
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            lock (WriteLock)
            {
                // Wrap
                writer.WriteStartObject();
                writer.WritePropertyName(_name);

                // Regular Serialization
                _canWrite = false;
                serializer.Serialize(writer, value);
                _canWrite = true;

                writer.WriteEndObject();
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            lock (ReadLock)
            {
                // Unwrap
                reader.Read(); // Advance past the initial start object token
                reader.Read(); // Advance past the property token

                // Regular Deserialization

                _canRead = false;
                var result = serializer.Deserialize(reader, objectType);
                _canRead = true;

                // Finish Unwrapping, otherwise json.net complains that the entire stream has not been read
                while (reader.Read()) { }

                return result;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}
