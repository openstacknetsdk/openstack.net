using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStack.ObjectStorage.v1.ContentObjectFilters {

	/// <summary>
	/// Define a filter to obtain items that are inside a specified range.
	/// <para>Items returned will be between <see cref="StartMarker"/> and <see cref="EndMarker"/>.</para>
	/// </summary>
	public class SearchRangeContentObjectFilter : ContentObjectFilterBase {
		
		/// <summary>
		/// The fullpath item to start search. If Null is considered as "not defined".
		/// <para>Note: selected item are fullpath greater than <see cref="StartMarker"/>.</para>
		/// </summary>
		public string StartMarker { get; set; }

		/// <summary>
		/// The fullpath item to end search. If Null is considered as "not defined".
		/// <para>Note: selected item are fullpath lower than <see cref="StartMarker"/>.</para>
		/// </summary>
		public string EndMarker { get; set; }


		/// <inheritdoc cref="IContentObjectFilter.CompileQueryParams"/>
		public override IEnumerable<KeyValuePair<string, string>> CompileQueryParams()
		{
			if (this.StartMarker != null)
			{
				yield return new KeyValuePair<string, string>("marker", this.StartMarker);
			}

			if (this.EndMarker != null)
			{
				yield return new KeyValuePair<string, string>("end_marker", this.EndMarker);
			}
		}
	}
}
