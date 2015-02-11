namespace OpenStack.Net
{
    using System;

    /// <summary>
    /// Represents the address family of a network address.
    /// </summary>
    /// <remarks>
    /// <para>This class allows the various builds of the SDK targeting different frameworks to use a consistent
    /// representation for an address family. For builds targeting frameworks that include their own representation of
    /// an address family, implicit operators are provided for converting that representation to and from instances of
    /// <see cref="AddressFamily"/>.</para>
    /// </remarks>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public struct AddressFamily : IEquatable<AddressFamily>
    {
        /// <summary>
        /// An <see cref="AddressFamily"/> representing an IP v4 address.
        /// </summary>
        public static readonly AddressFamily Ipv4 = new AddressFamily(1);

        /// <summary>
        /// An <see cref="AddressFamily"/> representing an IP v6 address.
        /// </summary>
        public static readonly AddressFamily Ipv6 = new AddressFamily(2);

        private readonly int _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressFamily"/> class with the specified address family value.
        /// </summary>
        /// <param name="value">The address family value.</param>
        private AddressFamily(int value)
        {
            _value = value;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (!(obj is AddressFamily))
                return false;

            return Equals((AddressFamily)obj);
        }

        /// <inheritdoc/>
        public bool Equals(AddressFamily other)
        {
            return _value == other._value;
        }

        /// <summary>
        /// Compares two <see cref="AddressFamily"/> instances for equality.
        /// </summary>
        /// <param name="x">The first <see cref="AddressFamily"/> instance.</param>
        /// <param name="y">The second <see cref="AddressFamily"/> instance.</param>
        /// <returns>
        /// <para><see langword="true"/> if <paramref name="x"/> and <paramref name="y"/> represent the same address
        /// family; otherwise, <see langword="false"/>.</para>
        /// </returns>
        public static bool operator ==(AddressFamily x, AddressFamily y)
        {
            return x.Equals(y);
        }

        /// <summary>
        /// Compares two <see cref="AddressFamily"/> instances for inequality.
        /// </summary>
        /// <param name="x">The first <see cref="AddressFamily"/> instance.</param>
        /// <param name="y">The second <see cref="AddressFamily"/> instance.</param>
        /// <returns>
        /// <para><see langword="true"/> if <paramref name="x"/> and <paramref name="y"/> represent different address
        /// families; otherwise, <see langword="false"/>.</para>
        /// </returns>
        public static bool operator !=(AddressFamily x, AddressFamily y)
        {
            return !x.Equals(y);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (this == Ipv4)
            {
                return "IPv4";
            }
            else if (this == Ipv6)
            {
                return "IPv6";
            }
            else
            {
                return "Unspecified";
            }
        }

#if WINRT
        /// <summary>
        /// Converts an <see cref="AddressFamily"/> to a <see cref="Windows.Networking.HostNameType"/>.
        /// </summary>
        /// <param name="addressFamily">The <see cref="AddressFamily"/> to convert.</param>
        /// <exception cref="NotSupportedException">If the specified <paramref name="addressFamily"/> does not have an
        /// equivalent <see cref="Windows.Networking.HostNameType"/> representation.</exception>
        /// <returns>A <see cref="Windows.Networking.HostNameType"/> which is equivalent to this
        /// <see cref="AddressFamily"/>.</returns>
        public static implicit operator Windows.Networking.HostNameType(AddressFamily addressFamily)
        {
            if (addressFamily == Ipv4)
                return Windows.Networking.HostNameType.Ipv4;
            else if (addressFamily == Ipv6)
                return Windows.Networking.HostNameType.Ipv6;
            else
                throw new NotSupportedException("Unsupported address family.");
        }

        /// <summary>
        /// Converts a <see cref="Windows.Networking.HostNameType"/> to an <see cref="AddressFamily"/>.
        /// </summary>
        /// <param name="addressFamily">The <see cref="Windows.Networking.HostNameType"/> to convert.</param>
        /// <returns>An <see cref="AddressFamily"/> which is equivalent to the specified
        /// <see cref="Windows.Networking.HostNameType"/>.</returns>
        /// <exception cref="NotSupportedException">If the specified <paramref name="addressFamily"/> does not have an
        /// equivalent <see cref="AddressFamily"/> representation.</exception>
        public static implicit operator AddressFamily(Windows.Networking.HostNameType addressFamily)
        {
            if (addressFamily == Windows.Networking.HostNameType.Ipv4)
                return Ipv4;
            else if (addressFamily == Windows.Networking.HostNameType.Ipv6)
                return Ipv6;
            else
                throw new NotSupportedException("Unsupported address family.");
        }
#elif !PORTABLE
        /// <summary>
        /// Converts an <see cref="AddressFamily"/> to an <see cref="System.Net.Sockets.AddressFamily"/>.
        /// </summary>
        /// <param name="addressFamily">The <see cref="AddressFamily"/> to convert.</param>
        /// <returns>An <see cref="System.Net.Sockets.AddressFamily"/> which is equivalent to this
        /// <see cref="AddressFamily"/>.</returns>
        public static implicit operator System.Net.Sockets.AddressFamily(AddressFamily addressFamily)
        {
            if (addressFamily == Ipv4)
                return System.Net.Sockets.AddressFamily.InterNetwork;
            else if (addressFamily == Ipv6)
                return System.Net.Sockets.AddressFamily.InterNetworkV6;
            else
                return System.Net.Sockets.AddressFamily.Unspecified;
        }

        /// <summary>
        /// Converts an <see cref="System.Net.Sockets.AddressFamily"/> to an <see cref="AddressFamily"/>.
        /// </summary>
        /// <param name="addressFamily">The <see cref="System.Net.Sockets.AddressFamily"/> to convert.</param>
        /// <returns>An <see cref="AddressFamily"/> which is equivalent to the specified
        /// <see cref="System.Net.Sockets.AddressFamily"/>.</returns>
        /// <exception cref="NotSupportedException">If the specified <paramref name="addressFamily"/> does not have an
        /// equivalent <see cref="AddressFamily"/> representation.</exception>
        public static implicit operator AddressFamily(System.Net.Sockets.AddressFamily addressFamily)
        {
            if (addressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                return Ipv4;
            else if (addressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                return Ipv6;
            else if (addressFamily == System.Net.Sockets.AddressFamily.Unspecified)
                return default(AddressFamily);
            else
                throw new NotSupportedException("Unsupported address family.");
        }
#endif
    }
}
