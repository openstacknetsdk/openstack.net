namespace OpenStack.Services.Networking.V2.LoadBalancer
{
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to <placeholder>UpdateMember</placeholder> with the OpenStack Networking Service V2.
    /// </summary>
    /// <seealso cref="ILoadBalancerExtension.PrepareUpdateMemberAsync"/>
    /// <seealso cref="LoadBalancerExtensions.UpdateMemberAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class UpdateMemberApiCall : DelegatingHttpApiCall<MemberResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateMemberApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="httpApiCall"/> is <see langword="null"/>.</exception>
        public UpdateMemberApiCall(IHttpApiCall<MemberResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
