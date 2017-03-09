using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

namespace OpenStack.ObjectStorage.v1.Metadata.ContainerMetadata {

	/// <summary>
	/// The `Access-Control-Allow-Origin` header metadata.
	/// </summary>
	public class AccessControlAllowOriginContainerMetadata : MetadataBase, IContainerMetadata
	{

		/// <summary>
		/// Create new instance
		/// </summary>
		public AccessControlAllowOriginContainerMetadata() : base("X-Container-Meta-Access-Control-Allow-Origin", true)
		{
			
		}
		
		/// <summary>
		/// Get or set list of allowed origins. Example: [ "http://web.com", "https://web.com" ]
		/// </summary>
		public string[] Origins
		{
			get { return MetadataConverter.ParseStringMultiValue(this.MetadataValue); }
			set { this.MetadataValue = MetadataConverter.SerializeStringValue(value); }
		}
		
	}
}
