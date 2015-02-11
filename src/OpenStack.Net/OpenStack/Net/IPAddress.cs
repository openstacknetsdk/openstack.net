namespace OpenStack.Net
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// Provides an Internet Protocol (IP) address.
    /// </summary>
    /// <remarks>
    /// <para>The <see cref="IPAddress"/> class contains the address of a computer on an IP network.</para>
    /// </remarks>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public sealed class IPAddress : IEquatable<IPAddress>
    {
        /// <summary>
        /// Provides an IP address that indicates that no network interface should be used. The <see cref="IPv6None"/>
        /// field is equivalent to <c>::</c> in compact notation.
        /// </summary>
        public static readonly IPAddress IPv6None = new IPAddress(new byte[16]);

        /// <summary>
        /// Provides an IP address that indicates that no network interface should be used. The <see cref="None"/>
        /// field is equivalent to <c>::</c> in compact notation.
        /// </summary>
        public static readonly IPAddress None = new IPAddress(0xFFFFFFFF);

        /// <summary>
        /// The address family for this address.
        /// </summary>
        private readonly AddressFamily _addressFamily;

        /// <summary>
        /// The IP v4 address. This field is only used when <see cref="_addressFamily"/> is
        /// <see cref="AddressFamily.Ipv4"/>.
        /// </summary>
        private readonly uint _address4;

        /// <summary>
        /// The IP v6 address. This field is only used when <see cref="_addressFamily"/> is
        /// <see cref="AddressFamily.Ipv6"/>.
        /// </summary>
        private readonly byte[] _address6;

        /// <summary>
        /// Initializes a new instance of the <see cref="IPAddress"/> class with the address specified as an
        /// <see cref="uint"/>.
        /// </summary>
        /// <param name="newAddress">The value of the IP address. For example, the value 0x2414188f in big-endian format
        /// would be the IP address "143.24.20.36".</param>
        public IPAddress(uint newAddress)
        {
            _addressFamily = AddressFamily.Ipv4;
            _address4 = newAddress;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IPAddress"/> class with the address specified as a
        /// <see cref="byte"/> array.
        /// </summary>
        /// <remarks>
        /// <para>If the length of <paramref name="address"/> is 4, <see cref="IPAddress(byte[])"/> constructs an IPv4
        /// address; otherwise, an IPv6 address is constructed.</para>
        ///
        /// <para>The <see cref="byte"/> array is assumed to be in network byte order with the most significant byte
        /// first in index position 0.</para>
        /// </remarks>
        /// <param name="address">The byte array value of the IP address.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="address"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="address"/> contains a bad IP address.</exception>
        public IPAddress(byte[] address)
        {
            if (address == null)
                throw new ArgumentNullException("address");
            if (address.Length != 4 && address.Length != 16)
                throw new ArgumentException("Bad IP address", "address");

            if (address.Length == 4)
            {
                _addressFamily = AddressFamily.Ipv4;
                _address4 = (uint)((address[3] << 24) | (address[2] << 16) | (address[1] << 8) | address[0]);
            }
            else
            {
                Debug.Assert(address.Length == 16);
                _addressFamily = AddressFamily.Ipv6;
                _address6 = (byte[])address.Clone();
            }
        }

        /// <summary>
        /// Gets the address family for this IP address.
        /// </summary>
        /// <value>
        /// An <see cref="Net.AddressFamily"/> representing the address family of the current IP address.
        /// </value>
        public AddressFamily AddressFamily
        {
            get
            {
                return _addressFamily;
            }
        }

        /// <summary>
        /// Provides a copy of the <see cref="IPAddress"/> as an array of bytes.
        /// </summary>
        /// <returns>A <see cref="byte"/> array.</returns>
        public byte[] GetAddressBytes()
        {
            if (_addressFamily == AddressFamily.Ipv4)
            {
                return new byte[] { (byte)_address4, (byte)(_address4 >> 8), (byte)(_address4 >> 16), (byte)(_address4 >> 24) };
            }
            else
            {
                return (byte[])_address6.Clone();
            }
        }

        /// <summary>
        /// Converts an IP address string to an <see cref="IPAddress"/> instance.
        /// </summary>
        /// <param name="ipString">A string that contains an IP address in dotted-quad notation for IPv4 and in
        /// colon-hexadecimal notation for IPv6.</param>
        /// <returns>An <see cref="IPAddress"/> instance.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="ipString"/> is <see langword="null"/>.</exception>
        /// <exception cref="FormatException">If <paramref name="ipString"/> is not a valid IP address.</exception>
        public static IPAddress Parse(string ipString)
        {
            if (ipString == null)
                throw new ArgumentNullException("ipString");

            IPAddress result;
            if (!TryParse(ipString, out result))
                throw new FormatException();

            return result;
        }

        /// <summary>
        /// Determines whether a string is a valid IP address.
        /// </summary>
        /// <param name="ipString">The string to validate.</param>
        /// <param name="address">The <see cref="IPAddress"/> version of the string.</param>
        /// <returns><see langword="true"/> if <paramref name="ipString"/> is a valid IP address; otherwise,
        /// <see langword="false"/>.</returns>
        public static bool TryParse(string ipString, out IPAddress address)
        {
            if (string.IsNullOrEmpty(ipString))
            {
                address = null;
                return false;
            }

            if (ipString.IndexOf(':') >= 0)
            {
                // IP v6

                if (ipString == "::")
                {
                    // this special case can't be parsed by the algorithm below
                    address = IPv6None;
                    return true;
                }

                List<string> parts = new List<string>(ipString.Split(':'));

                // The following sequence is not recommended per RFC 5952, but shows a case with 9 parts:
                //   ::0000:0000:0000:0000:0000:0000:0000
                if (parts.Count < 2 || parts.Count > 9)
                {
                    address = null;
                    return false;
                }

                if (parts[0] == string.Empty)
                {
                    if (parts[1] != string.Empty)
                    {
                        // started with :, but not ::
                        address = null;
                        return false;
                    }

                    parts.RemoveAt(0);
                }
                else if (parts[parts.Count - 1] == string.Empty)
                {
                    if (parts[parts.Count - 2] != string.Empty)
                    {
                        // ended with :, but not ::
                        address = null;
                        return false;
                    }

                    parts.RemoveAt(parts.Count - 1);
                }

                ushort[] segments = new ushort[8];
                bool foundEmpty = false;
                for (int i = 0; i < parts.Count; i++)
                {
                    if (parts[i] == string.Empty)
                    {
                        if (foundEmpty)
                        {
                            // multiple :: appeared...
                            address = null;
                            return false;
                        }

                        foundEmpty = true;
                        continue;
                    }

                    ushort value;
                    if (!ushort.TryParse(parts[i], NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out value))
                    {
                        address = null;
                        return false;
                    }

                    if (foundEmpty)
                    {
                        segments[segments.Length - (parts.Count - i)] = value;
                    }
                    else
                    {
                        segments[i] = value;
                    }
                }

                byte[] addressData = new byte[16];
                for (int i = 0; i < segments.Length; i++)
                {
                    addressData[2 * i + 1] = (byte)segments[i];
                    addressData[2 * i] = (byte)(segments[i] >> 8);
                }

                address = new IPAddress(addressData);
                return true;
            }
            else
            {
                string[] segments = ipString.Split('.');
                if (segments.Length != 4)
                {
                    address = null;
                    return false;
                }

                uint result = 0;
                for (int i = 0; i < segments.Length; i++)
                {
                    byte value;
                    if (!byte.TryParse(segments[i], NumberStyles.None, CultureInfo.InvariantCulture, out value))
                    {
                        address = null;
                        return false;
                    }

                    result |= (uint)(value << (8 * i));
                }

                address = new IPAddress(result);
                return true;
            }
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hash = 7;
            hash = 37 * hash ^ _addressFamily.GetHashCode();
            if (_addressFamily == AddressFamily.Ipv4)
            {
                hash = 37 * hash ^ _address4.GetHashCode();
            }
            else
            {
                for (int i = 0; i < _address6.Length; i++)
                    hash = 37 * hash ^ _address6[i].GetHashCode();
            }

            return hash;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            IPAddress other = obj as IPAddress;
            if (other == null)
                return false;

            return Equals(other);
        }

        /// <inheritdoc/>
        public bool Equals(IPAddress other)
        {
            if (other == null)
                return false;
            else if (ReferenceEquals(other, this))
                return true;

            if (_addressFamily != other._addressFamily)
                return false;

            if (_addressFamily == AddressFamily.Ipv4)
            {
                return _address4 == other._address4;
            }
            else
            {
                for (int i = 0; i < _address6.Length; i++)
                {
                    if (_address6[i] != other._address6[i])
                        return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Compares two <see cref="IPAddress"/> instances for equality.
        /// </summary>
        /// <param name="x">The first <see cref="IPAddress"/> instance.</param>
        /// <param name="y">The second <see cref="IPAddress"/> instance.</param>
        /// <returns>
        /// <para><see langword="true"/> if <paramref name="x"/> and <paramref name="y"/> represent the same address;
        /// otherwise, <see langword="false"/>.</para>
        /// </returns>
        public static bool operator ==(IPAddress x, IPAddress y)
        {
            if (ReferenceEquals(x, null))
                return ReferenceEquals(y, null);

            return x.Equals(y);
        }

        /// <summary>
        /// Compares two <see cref="IPAddress"/> instances for inequality.
        /// </summary>
        /// <param name="x">The first <see cref="IPAddress"/> instance.</param>
        /// <param name="y">The second <see cref="IPAddress"/> instance.</param>
        /// <returns>
        /// <para><see langword="true"/> if <paramref name="x"/> and <paramref name="y"/> represent different addresses;
        /// otherwise, <see langword="false"/>.</para>
        /// </returns>
        public static bool operator !=(IPAddress x, IPAddress y)
        {
            return !(x == y);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (_addressFamily == AddressFamily.Ipv4)
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}.{3}", _address4 & 0xFF, (_address4 >> 8) & 0xFF, (_address4 >> 16) & 0xFF, (_address4 >> 24) & 0xFF);
            }
            else
            {
                Debug.Assert(_addressFamily == AddressFamily.Ipv6);
                ushort[] data = new ushort[8];
                for (int i = 0; i < data.Length; i++)
                    data[i] = (ushort)((_address6[2 * i] << 8) | _address6[2 * i + 1]);

                int zeroSequenceStart;
                int zeroSequenceLength;
                FindLongestZeroSequence(data, out zeroSequenceStart, out zeroSequenceLength);

                StringBuilder result = new StringBuilder(5 * data.Length);
                for (int i = 0; i < data.Length; i++)
                {
                    if (i == zeroSequenceStart && zeroSequenceLength > 1)
                    {
                        result.Append("::");
                        continue;
                    }

                    if (i > zeroSequenceStart && i < zeroSequenceStart + zeroSequenceLength)
                    {
                        // suppressed
                        continue;
                    }

                    if (result.Length > 0 && result[result.Length - 1] != ':')
                    {
                        result.Append(':');
                    }

                    result.Append(data[i].ToString("x", CultureInfo.InvariantCulture));
                }

                return result.ToString();
            }
        }

        private void FindLongestZeroSequence(ushort[] data, out int start, out int length)
        {
            start = 0;
            length = 0;

            int currentStart = -1;
            int currentLength = 0;
            for (int i = 0; i <= data.Length; i++)
            {
                if (i < data.Length && data[i] == 0)
                {
                    if (currentLength == 0)
                        currentStart = i;

                    currentLength++;
                }
                else
                {
                    if (currentLength > length)
                    {
                        start = currentStart;
                        length = currentLength;
                    }

                    currentLength = 0;
                }
            }
        }

#if WINRT
        /// <summary>
        /// Converts an <see cref="IPAddress"/> to a <see cref="Windows.Networking.HostName"/>.
        /// </summary>
        /// <param name="ipAddress">The <see cref="IPAddress"/> to convert.</param>
        /// <returns>A <see cref="Windows.Networking.HostName"/> which is equivalent to this
        /// <see cref="IPAddress"/>.</returns>
        public static implicit operator Windows.Networking.HostName(IPAddress ipAddress)
        {
            if (ipAddress == null)
                return null;

            return new Windows.Networking.HostName(ipAddress.ToString());
        }

        /// <summary>
        /// Converts a <see cref="Windows.Networking.HostName"/> to an <see cref="IPAddress"/>.
        /// </summary>
        /// <param name="hostName">The <see cref="Windows.Networking.HostName"/> to convert.</param>
        /// <returns>An <see cref="IPAddress"/> which is equivalent to the specified
        /// <see cref="Windows.Networking.HostName"/>.</returns>
        /// <exception cref="NotSupportedException">If the specified <paramref name="hostName"/> does not have an
        /// equivalent <see cref="IPAddress"/> representation.</exception>
        public static implicit operator IPAddress(Windows.Networking.HostName hostName)
        {
            switch (hostName.Type)
            {
            case Windows.Networking.HostNameType.Ipv4:
            case Windows.Networking.HostNameType.Ipv6:
                return Parse(hostName.CanonicalName);

            default:
                throw new NotSupportedException(string.Format("The host name type '{0}' is not supported.", hostName.Type));
            }
        }
#elif !PORTABLE
        /// <summary>
        /// Converts an <see cref="IPAddress"/> to an <see cref="System.Net.IPAddress"/>.
        /// </summary>
        /// <param name="ipAddress">The <see cref="IPAddress"/> to convert.</param>
        /// <returns>An <see cref="System.Net.IPAddress"/> which is equivalent to this
        /// <see cref="IPAddress"/>.</returns>
        public static implicit operator System.Net.IPAddress(IPAddress ipAddress)
        {
            if (ipAddress == null)
                return null;

            if (ipAddress._addressFamily == AddressFamily.Ipv6)
                return new System.Net.IPAddress(ipAddress._address6);

            return new System.Net.IPAddress(ipAddress._address4);
        }

        /// <summary>
        /// Converts an <see cref="System.Net.IPAddress"/> to an <see cref="IPAddress"/>.
        /// </summary>
        /// <param name="ipAddress">The <see cref="System.Net.IPAddress"/> to convert.</param>
        /// <returns>An <see cref="IPAddress"/> which is equivalent to the specified
        /// <see cref="System.Net.IPAddress"/>.</returns>
        /// <exception cref="NotSupportedException">If the specified <paramref name="ipAddress"/> does not have an
        /// equivalent <see cref="IPAddress"/> representation.</exception>
        public static implicit operator IPAddress(System.Net.IPAddress ipAddress)
        {
            if (ipAddress == null)
                return null;

            if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
                || ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                return new IPAddress(ipAddress.GetAddressBytes());
            }
            else
            {
                throw new NotSupportedException();
            }
        }
#endif
    }
}
