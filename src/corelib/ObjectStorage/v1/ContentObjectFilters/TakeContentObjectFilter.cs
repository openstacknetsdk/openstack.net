using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStack.ObjectStorage.v1.ContentObjectFilters {

	/// <summary>
	/// Define a filter to obtain first # items.
	/// </summary>
	public class TakeContentObjectFilter : ContentObjectFilterBase {

		/// <summary>
		/// The limit of items to return.
		/// </summary>
		public int TakeLimit { get; set; }


		/// <inheritdoc cref="IContentObjectFilter.CompileQueryParams"/>
		public override IEnumerable<KeyValuePair<string, string>> CompileQueryParams()
		{
			yield return new KeyValuePair<string, string>("limit", this.TakeLimit.ToString("0", CultureInfo.InvariantCulture));
		}
	}
}
