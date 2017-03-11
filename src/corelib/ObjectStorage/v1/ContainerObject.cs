using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OpenStack.ObjectStorage.v1 {

	/// <summary>
	/// Represent an Object of container
	/// </summary>
	public class ContainerObject {

		/// <summary>
		/// Hash of Object
		/// </summary>
		[JsonProperty("hash")]
		public string Hash { get; set; }

		/// <summary>
		/// Date of last update
		/// </summary>
		[JsonProperty("last_modified")]
		public DateTime LastModified { get; set; }

		/// <summary>
		/// Weight of object
		/// </summary>
		[JsonProperty("bytes")]
		public long Bytes { get; set; }

		/// <summary>
		/// Object fullname
		/// </summary>
		[JsonProperty("name")]
		public string FullName { get; set; }

		/// <summary>
		/// Object content type
		/// </summary>
		[JsonProperty("content_type")]
		public string ContentType { get; set; }

	}
}
