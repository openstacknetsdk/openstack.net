namespace net.openstack.Core
{
    /// <summary>
    /// This interface provides methods for encoding and decoding strings which are
    /// embedded in the query string section of a URL.
    /// </summary>
    public interface IEncodeDecodeProvider
    {
        /// <summary>
        /// Encodes a string for inclusion in a URL.
        /// </summary>
        /// <remarks>
        /// The encoded string can be restored by calling <see cref="UrlDecode"/>.
        /// </remarks>
        /// <param name="stringToEncode">The string to encode.</param>
        /// <returns>The encoded string. If <paramref name="stringToEncode"/> is <c>null</c>, this method returns <c>null</c>.</returns>
        string UrlEncode(string stringToEncode);

        /// <summary>
        /// Decodes a string which is embedded in a URL.
        /// </summary>
        /// <param name="stringToDecode">The string to decode.</param>
        /// <returns>The decoded string. If <paramref name="stringToDecode"/> is <c>null</c>, this method returns <c>null</c>.</returns>
        string UrlDecode(string stringToDecode);
    }
}
