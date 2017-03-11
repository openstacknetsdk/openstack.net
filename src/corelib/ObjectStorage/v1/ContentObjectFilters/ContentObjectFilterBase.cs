using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStack.ObjectStorage.v1.ContentObjectFilters {

	/// <summary>
	/// Base class for ContentObjectFilter classes
	/// </summary>
	public abstract class ContentObjectFilterBase : IContentObjectFilter {

		/// <inheritdoc cref="IContentObjectFilter.CompileQueryParams"/>
		public abstract IEnumerable<KeyValuePair<string, string>> CompileQueryParams();

	}
}
