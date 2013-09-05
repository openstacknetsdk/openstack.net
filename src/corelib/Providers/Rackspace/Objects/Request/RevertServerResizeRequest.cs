namespace net.openstack.Providers.Rackspace.Objects.Request
{
    using Newtonsoft.Json;

    /// <summary>
    /// This models the JSON request used for the Revert Resized Server request.
    /// </summary>
    /// <seealso href="http://docs.openstack.org/api/openstack-compute/2/content/Revert_Resized_Server-d1e4024.html">Revert Resized Server (OpenStack Compute API v2 and Extensions Reference)</seealso>
    [JsonObject(MemberSerialization.OptIn)]
    internal class RevertServerResizeRequest
    {
#pragma warning disable 169 // The field 'fieldName' is never used
        [JsonProperty("revertResize", DefaultValueHandling = DefaultValueHandling.Include)]
        private string _command;
#pragma warning restore 169
    }
}
