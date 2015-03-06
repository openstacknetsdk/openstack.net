namespace OpenStack.Net
{
    using System;
    using System.Text;

    /// <summary>
    /// Provides the Media Access Control (MAC) address for a network interface (adapter).
    /// </summary>
    /// <remarks>
    /// <para>The MAC address, or physical address, is a hardware address that uniquely identifies each node, such as a
    /// computer or printer, on a network.</para>
    /// </remarks>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public sealed class PhysicalAddress
    {
        /// <summary>
        /// Returns a new <see cref="PhysicalAddress"/> instance with a zero length address. This field is read-only.
        /// </summary>
        public static readonly PhysicalAddress None = new PhysicalAddress(new byte[0]);

        private readonly byte[] _address;

        /// <summary>
        /// Initializes a new instance of the <see cref="PhysicalAddress"/> class.
        /// </summary>
        /// <remarks>
        /// <note>
        /// <para>Note that you can also use the <see cref="Parse"/> method to create a new instance of
        /// <see cref="PhysicalAddress"/>.</para>
        /// </note>
        /// </remarks>
        /// <param name="address">A <see cref="byte"/> array containing the address.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="address"/> is <see langword="null"/>.</exception>
        public PhysicalAddress(byte[] address)
        {
            if (address == null)
                throw new ArgumentNullException("address");

            _address = address;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hash = 0;

            int i;
            int size = _address.Length & ~3;

            for (i = 0; i < size; i += 4)
            {
                hash ^= (int)_address[i]
                        | ((int)_address[i + 1] << 8)
                        | ((int)_address[i + 2] << 16)
                        | ((int)_address[i + 3] << 24);
            }

            if ((_address.Length & 3) != 0)
            {
                int remnant = 0;
                int shift = 0;

                for (; i < _address.Length; ++i)
                {
                    remnant |= ((int)_address[i]) << shift;
                    shift += 8;
                }

                hash ^= remnant;
            }

            return hash;
        }

        /// <inheritdoc/>
        public override bool Equals(object comparand)
        {
            PhysicalAddress address = comparand as PhysicalAddress;
            if (address == null)
                return false;

            if (_address.Length != address._address.Length)
            {
                return false;
            }

            for (int i = 0; i < address._address.Length; i++)
            {
                if (_address[i] != address._address[i])
                    return false;
            }

            return true;
        }


        /// <inheritdoc/>
        public override string ToString()
        {
            StringBuilder addressString = new StringBuilder();
            foreach (byte value in _address)
            {
                int tmp = (value >> 4) & 0x0F;
                for (int i = 0; i < 2; i++)
                {
                    if (tmp < 0x0A)
                        addressString.Append((char)(tmp + 0x30));
                    else
                        addressString.Append((char)(tmp + 0x37));

                    tmp = value & 0x0F;
                }
            }

            return addressString.ToString();
        }

        /// <summary>
        /// Returns the address of the current instance.
        /// </summary>
        /// <returns>A <see cref="byte"/> array containing the address.</returns>
        public byte[] GetAddressBytes()
        {
            return (byte[])_address.Clone();
        }

        /// <summary>
        /// Parses the specified <see cref="string"/> and stores its contents as the address bytes of the
        /// <see cref="PhysicalAddress"/> returned by this method.
        /// </summary>
        /// <param name="address">
        /// A <see cref="string"/> containing the address that will be used to initialize the
        /// <see cref="PhysicalAddress"/> instance returned by this method.
        /// </param>
        /// <returns>A <see cref="PhysicalAddress"/> instance with the specified address.</returns>
        /// <exception cref="FormatException">
        /// <para>If the <paramref name="address"/> parameter contains an illegal hardware address.</para>
        /// <para>-or-</para>
        /// <para>If the <paramref name="address"/> parameter contains a string in the incorrect format.</para>
        /// </exception>
        public static PhysicalAddress Parse(string address)
        {
            int validCount = 0;
            bool hasDashes = false;
            byte[] buffer = null;

            if (address == null)
            {
                return PhysicalAddress.None;
            }

            //has dashes?
            if (address.IndexOf('-') >= 0)
            {
                hasDashes = true;
                buffer = new byte[(address.Length + 1) / 3];
            }
            else
            {

                if (address.Length % 2 > 0)
                {  //should be even
                    throw new FormatException("Bad MAC address");
                }

                buffer = new byte[address.Length / 2];
            }

            int j = 0;
            for (int i = 0; i < address.Length; i++)
            {

                int value = (int)address[i];

                if (value >= 0x30 && value <= 0x39)
                {
                    value -= 0x30;
                }
                else if (value >= 0x41 && value <= 0x46)
                {
                    value -= 0x37;
                }
                else if (value == (int)'-')
                {
                    if (validCount == 2)
                    {
                        validCount = 0;
                        continue;
                    }
                    else
                    {
                        throw new FormatException("Bad MAC address");
                    }
                }
                else
                {
                    throw new FormatException("Bad MAC address");
                }

                //we had too many characters after the last dash
                if (hasDashes && validCount >= 2)
                {
                    throw new FormatException("Bad MAC address");
                }

                if (validCount % 2 == 0)
                {
                    buffer[j] = (byte)(value << 4);
                }
                else
                {
                    buffer[j++] |= (byte)value;
                }

                validCount++;
            }

            //we too few characters after the last dash
            if (validCount < 2)
            {
                throw new FormatException("Bad MAC address");
            }

            return new PhysicalAddress(buffer);
        }

#if !PORTABLE
        /// <summary>
        /// Converts a <see cref="PhysicalAddress"/> to a <see cref="System.Net.NetworkInformation.PhysicalAddress"/>.
        /// </summary>
        /// <param name="physicalAddress">The <see cref="PhysicalAddress"/> to convert.</param>
        public static implicit operator System.Net.NetworkInformation.PhysicalAddress(PhysicalAddress physicalAddress)
        {
            if (physicalAddress == null)
                return null;

            return new System.Net.NetworkInformation.PhysicalAddress(physicalAddress.GetAddressBytes());
        }

        /// <summary>
        /// Converts a <see cref="System.Net.NetworkInformation.PhysicalAddress"/> to a <see cref="PhysicalAddress"/>.
        /// </summary>
        /// <param name="physicalAddress">The <see cref="System.Net.NetworkInformation.PhysicalAddress"/> to convert.</param>
        /// <exception cref="ArgumentException">If the specified <paramref name="physicalAddress"/> does not represent
        /// a physical address (i.e. <see cref="System.Net.NetworkInformation.PhysicalAddress.GetAddressBytes"/> returns
        /// <see langword="null"/>.</exception>
        public static implicit operator PhysicalAddress(System.Net.NetworkInformation.PhysicalAddress physicalAddress)
        {
            if (physicalAddress == null)
                return null;

            return new PhysicalAddress(physicalAddress.GetAddressBytes());
        }
#endif
    }
}
