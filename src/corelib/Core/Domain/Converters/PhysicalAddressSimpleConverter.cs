namespace net.openstack.Core.Domain.Converters
{
    using System.Net.NetworkInformation;
    using Newtonsoft.Json;

    /// <summary>
    /// This implementation of <see cref="JsonConverter"/> allows for JSON serialization
    /// and deserialization of <see cref="PhysicalAddress"/> objects using a simple string
    /// representation. Serialization is performed using <see cref="PhysicalAddress.ToString"/>,
    /// and deserialization is performed using <see cref="PhysicalAddress.Parse"/>.
    /// </summary>
    public class PhysicalAddressSimpleConverter : SimpleStringJsonConverter<PhysicalAddress>
    {
        /// <remarks>
        /// If <paramref name="str"/> is an empty string, this method returns <c>null</c>.
        /// Otherwise, this method uses <see cref="PhysicalAddress.Parse"/> for deserialization.
        /// </remarks>
        /// <inheritdoc/>
        protected override PhysicalAddress ConvertToObject(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            return PhysicalAddress.Parse(str);
        }
    }
}
