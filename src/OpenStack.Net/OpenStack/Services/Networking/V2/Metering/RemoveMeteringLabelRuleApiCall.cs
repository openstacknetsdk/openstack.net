namespace OpenStack.Services.Networking.V2.Metering
{
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to <placeholder>RemoveMeteringLabelRule</placeholder> with the OpenStack Networking Service V2.
    /// </summary>
    /// <seealso cref="IMeteringExtension.PrepareRemoveMeteringLabelRuleAsync"/>
    /// <seealso cref="MeteringExtensions.RemoveMeteringLabelRuleAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class RemoveMeteringLabelRuleApiCall : DelegatingHttpApiCall<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveMeteringLabelRuleApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="httpApiCall"/> is <see langword="null"/>.</exception>
        public RemoveMeteringLabelRuleApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
