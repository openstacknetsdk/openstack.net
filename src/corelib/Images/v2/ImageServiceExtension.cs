using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenStack.Images.v2;
using Flurl.Http;
using Flurl.Extensions;
using OpenStack.Images.v2.Serialization;
using OpenStack.Synchronous.Extensions;

namespace OpenStack.Images.v2.Synchronous
{
    /// <summary>
    ///Expose synchronous extensions methods for <seealso cref="ImageService"/> 
    /// </summary>
    public static class ImageServiceExtension
    {
        #region Image
        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public static IEnumerable<ImageOptions> ListImages(this ImageService service)
        {
            return service._imageApiBuilder.ListImageAsync().ForceSynchronous();
        }
        #endregion

    }
}
