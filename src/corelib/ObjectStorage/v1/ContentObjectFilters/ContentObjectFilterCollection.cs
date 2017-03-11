using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStack.ObjectStorage.v1.ContentObjectFilters {

	/// <summary>
	/// Collection of ContentObjectFilter
	/// </summary>
	public class ContentObjectFilterCollection : List<IContentObjectFilter> {

		/// <summary>
		/// Create new instance of ContentObjectFilter collection with preloaded items.
		/// </summary>
		public ContentObjectFilterCollection()
		{
			
		}

		/// <summary>
		/// Create new instance of ContentObjectFilter collection with preloaded items.
		/// </summary>
		/// <param name="collection"></param>
		public ContentObjectFilterCollection(IEnumerable<IContentObjectFilter> collection) : base(collection)
		{
			
		}


		/// <summary>
		/// Create QueryParams to append to URL
		/// </summary>
		/// <returns></returns>
		public IEnumerable<KeyValuePair<string, string>> CompileQueryParams()
		{
			foreach (var filter in this)
			{
				foreach (var compileQueryParam in filter.CompileQueryParams())
				{
					yield return compileQueryParam;
				}
			}
		}

	}
}
