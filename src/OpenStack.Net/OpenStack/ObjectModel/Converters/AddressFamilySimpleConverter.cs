namespace OpenStack.ObjectModel.Converters
{
    using System;
    using OpenStack.Net;

    /// <summary>
    /// This class supports deserializing an <see cref="AddressFamily"/> from the
    /// form commonly used in OpenStack services. In particular, the text <c>ipv4</c>
    /// is deserialized to <see cref="AddressFamily.Ipv4"/>, and <c>ipv6</c>
    /// is deserialized to <see cref="AddressFamily.Ipv6"/> (both are
    /// case-insensitive).
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class AddressFamilySimpleConverter : SimpleStringJsonConverter<AddressFamily?>
    {
        /// <inheritdoc/>
        protected override AddressFamily? ConvertToObject(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            switch (str.ToLowerInvariant())
            {
            case "ipv4":
                return AddressFamily.Ipv4;

            case "ipv6":
                return AddressFamily.Ipv6;

            default:
                throw new NotSupportedException("Unsupported address family: " + str);
            }
        }
    }
}
