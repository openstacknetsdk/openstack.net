namespace OpenStack.ObjectModel.Converters
{
#if WINRT
    using System;
    using HostNameType = Windows.Networking.HostNameType;
    using TAddressFamily = System.Nullable<Windows.Networking.HostNameType>;
#elif PORTABLE
    using TAddressFamily = System.String;
#else
    using System;
    using AddressFamily = System.Net.Sockets.AddressFamily;
    using TAddressFamily = System.Nullable<System.Net.Sockets.AddressFamily>;
#endif

    /// <summary>
    /// This class supports deserializing an <see cref="T:System.Net.Sockets.AddressFamily"/> from the
    /// form commonly used in OpenStack services. In particular, the text <c>ipv4</c>
    /// is deserialized to <see cref="F:System.Net.Sockets.AddressFamily.InterNetwork"/>, and <c>ipv6</c>
    /// is deserialized to <see cref="F:System.Net.Sockets.AddressFamily.InterNetworkV6"/> (both are
    /// case-insensitive).
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class AddressFamilySimpleConverter : SimpleStringJsonConverter<TAddressFamily>
    {
        /// <inheritdoc/>
        protected override TAddressFamily ConvertToObject(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

#if PORTABLE && !WINRT
            return str;
#else
            switch (str.ToLowerInvariant())
            {
            case "ipv4":
#if WINRT
                return HostNameType.Ipv4;
#else
                return AddressFamily.InterNetwork;
#endif

            case "ipv6":
#if WINRT
                return HostNameType.Ipv6;
#else
                return AddressFamily.InterNetworkV6;
#endif

            default:
                throw new NotSupportedException("Unsupported address family: " + str);
            }
#endif
        }
    }
}
