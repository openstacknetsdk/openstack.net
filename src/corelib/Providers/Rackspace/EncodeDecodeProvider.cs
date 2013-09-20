using System.Web;
using net.openstack.Core;

namespace net.openstack.Providers.Rackspace
{
    /// <summary>
    /// Provides a default implementation of <see cref="IEncodeDecodeProvider"/> for
    /// use with Rackspace services. This implementation uses <see cref="HttpUtility.UrlEncode(string)"/>
    /// and <see cref="HttpUtility.UrlDecode(string)"/>, with an additional string transformation that
    /// encodes the <c>+</c> character to <c>%20</c>, effectively rendering it to a space
    /// character.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
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

        /// <inheritdoc/>
        public string UrlEncode(string stringToEncode)
        {
            if (stringToEncode == null)
                return null;

            return HttpUtility.UrlEncode(stringToEncode).Replace("+","%20");
        }

        /// <inheritdoc/>
        public string UrlDecode(string stringToDecode)
        {
            if (stringToDecode == null)
                return null;

            return HttpUtility.UrlDecode(stringToDecode);
        }
    }
}
