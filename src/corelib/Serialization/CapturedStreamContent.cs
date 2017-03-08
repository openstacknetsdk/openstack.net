using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStack.Serialization {

	/// <summary>
	/// CaptureContent for stream data
	/// </summary>
	public class CapturedStreamContent : System.Net.Http.StreamContent {

		/// <summary>
		/// Creates new CaptureContent for stream
		/// </summary>
		/// <param name="dataStream"></param>
		public CapturedStreamContent(System.IO.Stream dataStream) : base(dataStream)
		{
			
		}
	}
}
