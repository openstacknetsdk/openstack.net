using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenStack.ObjectStorage.v1.Serialization;

namespace OpenStack.ObjectStorage.v1 {

	/// <summary>
	/// Result of BulkDelete operation.
	/// See <see cref="ObjectStorageService.DeleteContainerObjectListAsync(string, IEnumerable{string}, System.Threading.CancellationToken)"/>.
	/// </summary>
	public class BulkDeleteResult {

		/// <summary>
		/// Items found
		/// </summary>
		[JsonProperty("Number Not Found")]
		public int? NumberNotFound { get; set; }

		/// <summary>
		/// Items deleted
		/// </summary>
		[JsonProperty("Number Deleted")]
		public int? NumberDeleted { get; set; }

		/// <summary>
		/// Response status
		/// </summary>
		[JsonProperty("Response Status")]
        [JsonConverter(typeof(HttpStatusCodeConverter))]
		public System.Net.HttpStatusCode ResponseStatus { get; set; }

		/// <summary>
		/// Response body
		/// </summary>
		[JsonProperty("Response Body")]
		public string ResponseBody { get; set; }

		/// <summary>
		/// Errors
		/// </summary>
		[JsonProperty("Errors")]
		public ErrorItem[] Errors { get; set; }


		/// <summary>
		/// Error item
		/// </summary>
		public class ErrorItem
		{

			/// <summary>
			/// Item fullname
			/// </summary>
			[JsonProperty("Name")]
			public string Name { get; set; }
			
			/// <summary>
			/// Error status
			/// </summary>
			[JsonProperty("Status")]
			public string Status { get; set; }
		}

	}
}
