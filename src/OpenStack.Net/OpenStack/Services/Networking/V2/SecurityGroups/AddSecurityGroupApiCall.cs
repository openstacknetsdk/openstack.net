namespace OpenStack.Services.Networking.V2.SecurityGroups
{
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to <placeholder>AddSecurityGroup</placeholder> with the OpenStack Networking Service V2.
    /// </summary>
    /// <seealso cref="ISecurityGroupsExtension.PrepareAddSecurityGroupAsync"/>
    /// <seealso cref="SecurityGroupsExtensions.AddSecurityGroupAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class AddSecurityGroupApiCall : DelegatingHttpApiCall<SecurityGroupResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddSecurityGroupApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="httpApiCall"/> is <see langword="null"/>.</exception>
        public AddSecurityGroupApiCall(IHttpApiCall<SecurityGroupResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
