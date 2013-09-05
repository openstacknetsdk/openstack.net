namespace net.openstack.Providers.Rackspace.Objects.Request
{
    using Newtonsoft.Json;

    /// <summary>
    /// This models the JSON request used for the Unrescue Server request.
    /// </summary>
    /// <seealso href="http://docs.rackspace.com/servers/api/v2/cs-devguide/content/exit_rescue_mode.html">Unrescue Server (Rackspace Next Generation Cloud Servers Developer Guide - API v2)</seealso>
    [JsonObject(MemberSerialization.OptIn)]
    internal class UnrescueServerRequest
    {
#pragma warning disable 169 // The field 'fieldName' is never used
        [JsonProperty("unrescue", DefaultValueHandling = DefaultValueHandling.Include)]
        private string _command;
#pragma warning restore 169
    }
}