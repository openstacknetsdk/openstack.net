using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
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
			if (string.IsNullOrEmpty(serializedValue)) return Enumerable.Empty<T>();
			
			var re = new Regex(@"\s*(?<key>[a-zA-Z\-_]+)\s*:\s*(?<value>[^\n\r]*)");
			var matches = re.Matches(serializedValue);

			return this.ParseMetadataHeaders(
				matches
					.Cast<System.Text.RegularExpressions.Match>()
					.Select(match => new KeyValuePair<string, IEnumerable<string>>(
						match.Groups["key"].Value,
						match.Groups["value"].Value
							.Trim()
							.Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries)
							.Select(item => item.Trim())
							.Where(item => string.IsNullOrEmpty(item) == false)
					))
			);
		}

		/// <summary>
		/// Parse HttpResponseHeaders to Metadata collection
		/// </summary>
		/// <param name="headers"></param>
		/// <returns></returns>
		public IEnumerable<T> ParseMetadataHeaders(HttpResponseHeaders headers)
		{
			return this.ParseMetadataHeaders(headers.Select(item => item));
		}

		/// <summary>
		/// Parse value to Metadata collection
		/// </summary>
		/// <param name="headers"></param>
		/// <returns></returns>
		public IEnumerable<T> ParseMetadataHeaders(IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers)
		{
			var metadataTypes = this.getCompatibleMetadataTypeList().ToArray();

			foreach (var header in headers)
			{
				var metadataInfo = metadataTypes
					.FirstOrDefault(item => item.MetadataKey.Equals(header.Key, StringComparison.InvariantCultureIgnoreCase));
				if (metadataInfo == null) continue;

				var metadata = (T)System.Activator.CreateInstance(metadataInfo.MetadataType);
				
				if (metadata.AllowMultiValue == false)
				{
					metadata.MetadataValue = header.Value
						.Select(item => item.Trim())
						.Where(item => string.IsNullOrEmpty(item) == false)
						.ToArray();
				}
				else
				{
					metadata.MetadataValue = header.Value
						.SelectMany(item => item.Split(new []{','}, StringSplitOptions.RemoveEmptyEntries))
						.Select(item => item.Trim())
						.Where(item => string.IsNullOrEmpty(item) == false)
						.ToArray();
				}

				yield return metadata;
			}
		}


		private IEnumerable<MetadataInfo> getCompatibleMetadataTypeList()
		{
			var metadataTypeInterface = typeof(T);
			var metadataTypes = System.Reflection.Assembly.GetCallingAssembly()
				.GetTypes()
				.Where(classType => classType.IsClass && !classType.IsAbstract && metadataTypeInterface.IsAssignableFrom(classType))
				.Select(classType => new MetadataInfo()
				{
					MetadataType = classType,
					MetadataKey = ((T) System.Activator.CreateInstance(classType)).MetadataKey
				});
			return metadataTypes;
		}

		private class MetadataInfo
		{
			
			public Type MetadataType { get; set; }

			public string MetadataKey { get; set; }
		}
	}
}
