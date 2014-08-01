using Newtonsoft.Json;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects.Response
{
    /// <summary>
    /// This models the JSON response used for the Get Endpoint request.
    /// </summary>
    /// <seealso href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/GET_getEndpoint__v2.0_tenants__tenantId__OS-KSCATALOG_endpoints__endpointId__Endpoint_Operations_OS-KSCATALOG.html">List Service Catalog Endpoints (OpenStack Identity Service API v2.0 Reference)</seealso>
    /// <threadsafety static="true" instance="false"/>
    [JsonObject(MemberSerialization.OptIn)]
    internal class GetEndpointResponse
    {
        /// <summary>
        /// Gets additional information about the endpoint.
        /// </summary>
        /// <seealso cref="UserAccess"/>
        [JsonProperty("endpoint")]
        public ExtendedEndpoint Endpoint { get; private set; }
    }
}