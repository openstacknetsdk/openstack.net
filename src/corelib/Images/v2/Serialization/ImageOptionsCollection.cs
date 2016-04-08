using OpenStack.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStack.Images.v2.Serialization
{

    /// <summary>
    ///Represent collection of the <seealso cref="ImageOptions"/> 
    /// </summary>
    [JsonConverterWithConstructor(typeof(RootWrapperConverter), "images")]
    public class ImageOptionsCollection : List<ImageOptions>
    {
        /// <summary>
        ///initializes the new instance of <seealso cref="ImageOptionsCollection"/>
        /// </summary>
        public ImageOptionsCollection()
        {
        }

        /// <summary>
        ///initializes the new instance of <seealso cref="ImageOptionsCollection"/>
        /// </summary>
        /// <param name="networks"></param>
        public ImageOptionsCollection(IEnumerable<ImageOptions> networks) : base(networks)
        {
        }

    }
}
