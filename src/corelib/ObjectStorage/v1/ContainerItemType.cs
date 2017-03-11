using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStack.ObjectStorage.v1 {

	/// <summary>
	/// List of ContainerItem types
	/// </summary>
	public enum ContainerItemType {

		/// <summary>
		/// ContainerObject, like a file
		/// </summary>
		Object = 1,

		/// <summary>
		/// ContainerDirectoty, like a folder
		/// </summary>
		Directory = 2

	}
}
