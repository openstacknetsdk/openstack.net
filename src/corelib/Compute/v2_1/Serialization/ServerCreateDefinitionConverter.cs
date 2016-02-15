using System;
using Newtonsoft.Json;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1.Serialization
{
    /// <summary>
    /// Serializes server create requests, which have an odd nesting structure.
    /// </summary>
    /// <exclude />
    public class ServerCreateDefinitionConverter : DefaultJsonConverter
    {
        /// <inheritdoc />
        public override bool CanRead => false;

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            // Serialize server
            writer.WritePropertyName("server");
            base.WriteJson(writer, value, serializer);

            // Serialize scheduler hints
            var server = value as ServerCreateDefinition;
            if (server?.SchedulerHints != null)
            {
                writer.WritePropertyName("os:scheduler_hints");
                base.WriteJson(writer, server.SchedulerHints, serializer);
            }

            writer.WriteEndObject();
        }

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return typeof (ServerCreateDefinition).IsAssignableFrom(objectType);
        }
    }
}