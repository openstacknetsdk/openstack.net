namespace OpenStack.Services.Networking.V2.Metering
{
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to <placeholder>RemoveMeteringLabel</placeholder> with the OpenStack Networking Service V2.
    /// </summary>
    /// <seealso cref="IMeteringExtension.PrepareRemoveMeteringLabelAsync"/>
    /// <seealso cref="MeteringExtensions.RemoveMeteringLabelAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class RemoveMeteringLabelApiCall : DelegatingHttpApiCall<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveMeteringLabelApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="httpApiCall"/> is <see langword="null"/>.</exception>
        public RemoveMeteringLabelApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
