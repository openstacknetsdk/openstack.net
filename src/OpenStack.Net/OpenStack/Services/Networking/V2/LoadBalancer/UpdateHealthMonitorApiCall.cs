﻿namespace OpenStack.Services.Networking.V2.LoadBalancer
{
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to <placeholder>UpdateHealthMonitor</placeholder> with the OpenStack Networking Service V2.
    /// </summary>
    /// <seealso cref="ILoadBalancerExtension.PrepareUpdateHealthMonitorAsync"/>
    /// <seealso cref="LoadBalancerExtensions.UpdateHealthMonitorAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class UpdateHealthMonitorApiCall : DelegatingHttpApiCall<HealthMonitorResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateHealthMonitorApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="httpApiCall"/> is <see langword="null"/>.</exception>
        public UpdateHealthMonitorApiCall(IHttpApiCall<HealthMonitorResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
