namespace OpenStack.Services.Networking.V2.SecurityGroups
{
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to <placeholder>GetSecurityGroup</placeholder> with the OpenStack Networking Service V2.
    /// </summary>
    /// <seealso cref="ISecurityGroupsExtension.PrepareGetSecurityGroupAsync"/>
    /// <seealso cref="SecurityGroupsExtensions.GetSecurityGroupAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class GetSecurityGroupApiCall : DelegatingHttpApiCall<SecurityGroupResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetSecurityGroupApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="httpApiCall"/> is <see langword="null"/>.</exception>
        public GetSecurityGroupApiCall(IHttpApiCall<SecurityGroupResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
