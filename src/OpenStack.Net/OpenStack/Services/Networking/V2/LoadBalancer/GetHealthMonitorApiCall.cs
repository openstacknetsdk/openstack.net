namespace OpenStack.Services.Networking.V2.LoadBalancer
{
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to <placeholder>GetHealthMonitor</placeholder> with the OpenStack Networking Service V2.
    /// </summary>
    /// <seealso cref="ILoadBalancerExtension.PrepareGetHealthMonitorAsync"/>
    /// <seealso cref="LoadBalancerExtensions.GetHealthMonitorAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class GetHealthMonitorApiCall : DelegatingHttpApiCall<HealthMonitorResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetHealthMonitorApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="httpApiCall"/> is <see langword="null"/>.</exception>
        public GetHealthMonitorApiCall(IHttpApiCall<HealthMonitorResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
