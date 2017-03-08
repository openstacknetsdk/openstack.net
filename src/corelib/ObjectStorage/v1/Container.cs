using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OpenStack.ObjectStorage.v1 {

	/// <summary>
	/// Represents a Container
	/// </summary>
	public class Container {

		/// <summary>
		/// Container name
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; set; }

		/// <summary>
		/// Count of objects in container
		/// </summary>
		[JsonProperty("count")]
		public long Count { get; set; }

		/// <summary>
		/// Weight of container
		/// </summary>
		[JsonProperty("bytes")]
		public long Bytes { get; set; }

	}
}
