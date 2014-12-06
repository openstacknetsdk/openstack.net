namespace OpenStack.Services.Networking.V2.LoadBalancer
{
    using OpenStack.Collections;
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to <placeholder>ListMembers</placeholder> with the OpenStack Networking Service V2.
    /// </summary>
    /// <seealso cref="ILoadBalancerExtension.PrepareListMembersAsync"/>
    /// <seealso cref="LoadBalancerExtensions.ListMembersAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class ListMembersApiCall : DelegatingHttpApiCall<ReadOnlyCollectionPage<Member>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListMembersApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="httpApiCall"/> is <see langword="null"/>.</exception>
        public ListMembersApiCall(IHttpApiCall<ReadOnlyCollectionPage<Member>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
