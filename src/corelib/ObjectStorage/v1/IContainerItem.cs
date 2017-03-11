using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenStack.ObjectStorage.v1.Serialization;

namespace OpenStack.ObjectStorage.v1 {

	/// <summary>
	/// Represents a item of container.
	/// </summary>
	[JsonConverter(typeof(ContainerItemConverter))]
	public interface IContainerItem {

		/// <summary>
		/// Fullname of item
		/// </summary>
		string FullName { get; set; }

		/// <summary>
		/// Indicates the type of item
		/// </summary>
		ContainerItemType ItemType { get; }

	}
}
