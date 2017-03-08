using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStack.ObjectStorage.v1.Serialization {

	/// <summary>
	/// Represents a generic serialized collection's item 
	/// </summary>
	public interface ISerializedKeyValuePair {

		/// <summary>
		/// Key of item
		/// </summary>
		string Key { get; set; }

		/// <summary>
		/// Value of item
		/// </summary>
		string Value { get; set; }

	}
}
