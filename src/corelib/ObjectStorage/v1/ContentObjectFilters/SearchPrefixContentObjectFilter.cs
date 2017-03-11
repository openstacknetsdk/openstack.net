using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStack.ObjectStorage.v1.ContentObjectFilters {

	/// <summary>
	/// Define a filter to obtain items where fullname starts with prefix.
	/// <para>Alert: do not work when combined with <see cref="PathContentObjectFilter"/>.</para>
	/// </summary>
	public class SearchPrefixContentObjectFilter : ContentObjectFilterBase {

		/// <summary>
		/// The prefix of fullname items that will be return.
		/// </summary>
		public string PathPrefix { get; set; }


		/// <inheritdoc cref="IContentObjectFilter.CompileQueryParams"/>
		public override IEnumerable<KeyValuePair<string, string>> CompileQueryParams()
		{
			yield return new KeyValuePair<string, string>("prefix", this.PathPrefix);
		}
	}
}
