using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenStack.ObjectStorage.v1.Metadata;

namespace OpenStack.ObjectStorage.v1.Serialization {

	/// <summary>
	/// Stringify and parse Metadata
	/// </summary>
	public class MetadataSerializer<T> where T : IMetadata {

		/// <summary>
		/// Stringify the Metadata collection.
		/// </summary>
		/// <param name="metadataCollection"></param>
		/// <returns></returns>
		public string StringifyMetadataHeaderStyle(IEnumerable<T> metadataCollection)
		{
			if (metadataCollection == null) return "";

			var fReturn = new StringBuilder();

			foreach (var metadata in metadataCollection)
			{
				fReturn.AppendLine(string.Format("{0}: {1}", metadata.MetadataKey, metadata.MetadataValue));
			}

			return fReturn.ToString();
		}

		/// <summary>
		/// Parse value to Metadata collection
		/// </summary>
		/// <param name="serializedValue">Serialized value of Metadata collection</param>
		/// <returns></returns>
		public IEnumerable<T> ParseMetadataHeaderStyle(string serializedValue)
		{
			if (string.IsNullOrEmpty(serializedValue)) yield break;

			var metadataTypeInterface = typeof(T);
			var metadataTypes = System.Reflection.Assembly.GetExecutingAssembly()
				.GetTypes()
				.Where(classType => classType.IsClass && !classType.IsAbstract && metadataTypeInterface.IsAssignableFrom(classType))
				.Select(classType => new { type = classType, metadataKey = ((T)System.Activator.CreateInstance(classType)).MetadataKey })
				.ToArray();

			var re = new Regex(@"\s*(?<key>[a-zA-Z\-\_]+)\s*:\s*(?<value>[^\n\r]*)");
			var matches = re.Matches(serializedValue);

			foreach (Match match in matches)
			{
				var key = match.Groups["key"].Value;
				var value = match.Groups["value"].Value.Trim();

				var metadataInfo = metadataTypes
					.FirstOrDefault(item => item.metadataKey.Equals(key, StringComparison.InvariantCultureIgnoreCase));
				if (metadataInfo == null) continue;

				var metadata = (T)System.Activator.CreateInstance(metadataInfo.type);
				metadata.MetadataValue = value;

				yield return metadata;
			}
		}

	}
}
