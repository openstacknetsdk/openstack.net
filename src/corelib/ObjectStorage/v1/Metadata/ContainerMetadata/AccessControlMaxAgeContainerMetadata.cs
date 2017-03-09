using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStack.ObjectStorage.v1.Metadata.ContainerMetadata {

	/// <summary>
	/// The `Access-Control-Max-Age` header metadata.
	/// </summary>
	public class AccessControlMaxAgeContainerMetadata : MetadataBase, IContainerMetadata {

		/// <summary>
		/// Create new instance
		/// </summary>
		public AccessControlMaxAgeContainerMetadata() : base("X-Container-Meta-Access-Control-Max-Age", false)
		{
			
		}
		
		/// <summary>
		/// Get or set the max age of container objects, in seconds.
		/// </summary>
		public long MaxAgeSeconds
		{
			get { return MetadataConverter.ParseLongSingleValue(this.MetadataValue); }
			set { this.MetadataValue = MetadataConverter.SerializeLongValue(value); }
		}
		
	}
}
