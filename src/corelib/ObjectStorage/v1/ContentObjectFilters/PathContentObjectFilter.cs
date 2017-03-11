using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStack.ObjectStorage.v1.ContentObjectFilters {
	
	/// <summary>
	/// Define a filter to obtain items in specified container path.
	/// </summary>
	public class PathContentObjectFilter : ContentObjectFilterBase {

		/// <summary>
		/// The path of container.
		/// </summary>
		public string Path { get; set; }


		/// <inheritdoc cref="IContentObjectFilter.CompileQueryParams"/>
		public override IEnumerable<KeyValuePair<string, string>> CompileQueryParams()
		{
			yield return new KeyValuePair<string, string>("path", this.Path);
		}
	}
}
