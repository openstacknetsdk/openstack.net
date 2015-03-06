namespace OpenStack.Services.Networking.V2.PortsBinding
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;
    using OpenStack.Net;
    using OpenStack.ObjectModel;
    using Rackspace.Threading;
    using ExtensionAlias = Identity.V2.ExtensionAlias;

    public static class PortsBindingExtensions
    {
        public static readonly ExtensionAlias ExtensionAlias = new ExtensionAlias("ports");

        public static Task<bool> SupportsPortsBindingAsync(this INetworkingService client, CancellationToken cancellationToken)
        {
            return client.ListExtensionsAsync(cancellationToken)
                .Select(task => task.Result.Any(i => ExtensionAlias.Equals(i.Alias)));
        }

        public static string GetBindingVifType(this PortData port)
        {
            if (port == null)
                throw new ArgumentNullException("port");

            JToken value;
            if (!port.ExtensionData.TryGetValue("binding:vif_type", out value))
                return null;

            return value.ToObject<string>();
        }

        public static HostId GetBindingHostId(this PortData port)
        {
            if (port == null)
                throw new ArgumentNullException("port");

            JToken value;
            if (!port.ExtensionData.TryGetValue("binding:host_id", out value))
                return null;

            return value.ToObject<HostId>();
        }

        public static JObject GetBindingProfile(this PortData port)
        {
            if (port == null)
                throw new ArgumentNullException("port");

            JToken value;
            if (!port.ExtensionData.TryGetValue("binding:profile", out value))
                return null;

            return (JObject)value;
        }

        public static BindingCapabilities GetBindingCapabilities(this PortData port)
        {
            if (port == null)
                throw new ArgumentNullException("port");

            JToken value;
            if (!port.ExtensionData.TryGetValue("binding:capabilities", out value))
                return null;

            return value.ToObject<BindingCapabilities>();
        }

        public static VnicType GetBindingVnicType(this PortData port)
        {
            if (port == null)
                throw new ArgumentNullException("port");

            JToken value;
            if (!port.ExtensionData.TryGetValue("binding:vnic_type", out value))
                return null;

            return value.ToObject<VnicType>();
        }

        public static PortData WithBindingVifType(this PortData port, string vifType)
        {
            var extensionData = port.ExtensionData.SetItem("binding:vif_type", vifType);
            return port.WithExtensionData(extensionData);
        }

        public static PortData WithBindingHostId(this PortData port, HostId hostId)
        {
            var extensionData = port.ExtensionData.SetItem("binding:host_id", JToken.FromObject(hostId));
            return port.WithExtensionData(extensionData);
        }

        public static PortData WithBindingProfile(this PortData port, JObject profile)
        {
            var extensionData = port.ExtensionData.SetItem("binding:profile", profile);
            return port.WithExtensionData(extensionData);
        }

        public static PortData WithBindingCapabilities(this PortData port, BindingCapabilities capabilities)
        {
            var extensionData = port.ExtensionData.SetItem("binding:capabilities", JToken.FromObject(capabilities));
            return port.WithExtensionData(extensionData);
        }

        public static PortData WithBindingVnicType(this PortData port, VnicType vnicType)
        {
            var extensionData = port.ExtensionData.SetItem("binding:vnic_type", JToken.FromObject(vnicType));
            return port.WithExtensionData(extensionData);
        }
    }
}
