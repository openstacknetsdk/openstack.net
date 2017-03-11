using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStack.ObjectStorage.v1.ContentObjectFilters {

	/// <summary>
	/// Define a filter that lock search inside folder when find <see cref="Delimiter"/> string in item fullname.
	/// <para>When combined with <see cref="SearchPrefixContentObjectFilter"/> it returns Subfolders.</para>
	/// </summary>
	public class DelimiterContentObjectFilter : ContentObjectFilterBase {

		/// <summary>
		/// Create new instance of filter and preload properties.
		/// </summary>
		public DelimiterContentObjectFilter()
		{
			this.Delimiter = "/";
		}

		/// <summary>
		/// The delimiter string to find in item fullname.
		/// </summary>
		public string Delimiter { get; set; }


		/// <inheritdoc cref="IContentObjectFilter.CompileQueryParams"/>
		public override IEnumerable<KeyValuePair<string, string>> CompileQueryParams()
		{
			yield return new KeyValuePair<string, string>("delimiter", this.Delimiter);
		}
	}
}
