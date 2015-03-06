namespace OpenStack.Services.Networking.V2.SecurityGroups
{
    using OpenStack.Collections;
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to <placeholder>ListSecurityGroups</placeholder> with the OpenStack Networking Service V2.
    /// </summary>
    /// <seealso cref="ISecurityGroupsExtension.PrepareListSecurityGroupsAsync"/>
    /// <seealso cref="SecurityGroupsExtensions.ListSecurityGroupsAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class ListSecurityGroupsApiCall : DelegatingHttpApiCall<ReadOnlyCollectionPage<SecurityGroup>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListSecurityGroupsApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="httpApiCall"/> is <see langword="null"/>.</exception>
        public ListSecurityGroupsApiCall(IHttpApiCall<ReadOnlyCollectionPage<SecurityGroup>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
