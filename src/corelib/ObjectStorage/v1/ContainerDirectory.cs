using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OpenStack.ObjectStorage.v1 {

	/// <summary>
	/// Represent an Object of container
	/// </summary>
	public class ContainerDirectory : IContainerItem {

		/// <summary>
		/// Directory fullname
		/// </summary>
		[JsonProperty("subdir")]
		public string FullName { get; set; }

		/// <inheritdoc cref="IContainerItem.ItemType"/>
		public ContainerItemType ItemType { get { return ContainerItemType.Directory; } }
	}
}
