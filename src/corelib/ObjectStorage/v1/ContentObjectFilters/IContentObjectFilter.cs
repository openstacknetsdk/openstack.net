using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStack.ObjectStorage.v1.ContentObjectFilters {

	/// <summary>
	/// Define a filter for Content Object search
	/// </summary>
	public interface IContentObjectFilter
	{

		/// <summary>
		/// Create query parameters to append to URL
		/// </summary>
		/// <returns></returns>
		IEnumerable<KeyValuePair<string, string>> CompileQueryParams();

	}
}
