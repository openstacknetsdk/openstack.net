using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using OpenStack.Networking.v2;

namespace OpenStack.ObjectStorage.v1.Serialization
{
    /// <summary>
    /// Handles serialization/deserialization for <see cref="PortCreateDefinition.DHCPOptions"/>.
    /// </summary>
    /// <seealso href="http://specs.openstack.org/openstack/neutron-specs/specs/api/extra_dhcp_options__extra-dhcp-opt_.html"/>
    /// <exclude />
    public class ContainerItemConverter : JsonConverter
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
			
            var containerItem = (IContainerItem) value;
	        switch (containerItem.ItemType)
	        {
				case ContainerItemType.Directory:
					writer.WriteValue((ContainerDirectory)value);
					break;
				case ContainerItemType.Object:
					writer.WriteValue((ContainerObject)value);
					break;
				default:
					throw new InvalidEnumArgumentException();
	        }
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
            if (reader.TokenType == JsonToken.StartObject)
                ReadAndAssert(reader);

			if (reader.TokenType != JsonToken.PropertyName)
				throw new JsonSerializationException("Unexpected end when reading IContainerItem.");

	        var firstPropertyName = reader.Value.ToString();
	        switch (firstPropertyName)
	        {
				case "subdir":
					// Is a directory
			        return readDirectory(reader, serializer);
				case "bytes":
				case "content_type":
				case "hash":
				case "last_modified":
				case "name":
					// Is an object
			        return readObject(reader, serializer);
				default:
					throw new JsonSerializationException("Unexpected end when reading IContainerItem.");
	        }
        }

	    private static ContainerDirectory readDirectory(JsonReader reader, JsonSerializer serializer)
	    {
		    var dirItem = new ContainerDirectory();
			
	        while (reader.TokenType == JsonToken.PropertyName)
	        {
				var propertyName = reader.Value.ToString();
				ReadAndAssert(reader);
		        switch (propertyName)
		        {
					case "subdir":
				        dirItem.FullName = serializer.Deserialize<string>(reader);
						break;
		        }

				ReadAndAssert(reader);
	        }

			return dirItem;
	    }

        private static ContainerObject readObject(JsonReader reader, JsonSerializer serializer)
        {
            var objItem = new ContainerObject();
			
	        while (reader.TokenType == JsonToken.PropertyName)
	        {
				var propertyName = reader.Value.ToString();
				ReadAndAssert(reader);
		        switch (propertyName)
		        {
					case "bytes":
				        objItem.Bytes = serializer.Deserialize<long>(reader);
						break;
					case "content_type":
				        objItem.ContentType = serializer.Deserialize<string>(reader);
						break;
					case "hash":
				        objItem.Hash = serializer.Deserialize<string>(reader);
						break;
					case "last_modified":
				        var date = serializer.Deserialize<DateTime>(reader);
				        objItem.LastModified = new DateTime(date.Ticks, DateTimeKind.Utc);
						break;
					case "name":
				        objItem.FullName = serializer.Deserialize<string>(reader);
						break;
		        }

				ReadAndAssert(reader);
	        }

            return objItem;
        } 

        private static void ReadAndAssert(JsonReader reader)
        {
            if (!reader.Read())
                throw new JsonSerializationException("Unexpected end when reading IContainerItem.");
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof (IContainerItem).IsAssignableFrom(objectType);
        }
    }
}