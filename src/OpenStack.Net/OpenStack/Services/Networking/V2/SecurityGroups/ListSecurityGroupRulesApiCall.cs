namespace OpenStack.Services.Networking.V2.SecurityGroups
{
    using OpenStack.Collections;
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to <placeholder>ListSecurityGroupRules</placeholder> with the OpenStack Networking Service V2.
    /// </summary>
    /// <seealso cref="ISecurityGroupsExtension.PrepareListSecurityGroupRulesAsync"/>
    /// <seealso cref="SecurityGroupsExtensions.ListSecurityGroupRulesAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class ListSecurityGroupRulesApiCall : DelegatingHttpApiCall<ReadOnlyCollectionPage<SecurityGroupRule>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListSecurityGroupRulesApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="httpApiCall"/> is <see langword="null"/>.</exception>
        public ListSecurityGroupRulesApiCall(IHttpApiCall<ReadOnlyCollectionPage<SecurityGroupRule>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
