using System.Web;
using net.openstack.Core;

namespace net.openstack.Providers.Rackspace
{
    internal class EncodeDecodeProvider : IEncodeDecodeProvider
    {
        /// <summary>
        /// A default instance of <see cref="EncodeDecodeProvider"/>.
        /// </summary>
        private static readonly EncodeDecodeProvider _default = new EncodeDecodeProvider();

        /// <summary>
        /// Gets a default instance of <see cref="EncodeDecodeProvider"/>.
        /// </summary>
        public static EncodeDecodeProvider Default
        {
            get
            {
                return _default;
            }
        }

        public string UrlEncode(string stringToEncode)
        {
            return HttpUtility.UrlEncode(stringToEncode).Replace("+","%20");
        }

        public string UrlDecode(string stringToDecode)
        {
            return HttpUtility.UrlDecode(stringToDecode);
        }
    }
}
