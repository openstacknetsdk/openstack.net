namespace OpenStack.Services.Networking.V2.Metering
{
    using OpenStack.Collections;
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to <placeholder>ListMeteringLabelRules</placeholder> with the OpenStack Networking Service V2.
    /// </summary>
    /// <seealso cref="IMeteringExtension.PrepareListMeteringLabelRulesAsync"/>
    /// <seealso cref="MeteringExtensions.ListMeteringLabelRulesAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class ListMeteringLabelRulesApiCall : DelegatingHttpApiCall<ReadOnlyCollectionPage<MeteringLabelRule>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListMeteringLabelRulesApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="httpApiCall"/> is <see langword="null"/>.</exception>
        public ListMeteringLabelRulesApiCall(IHttpApiCall<ReadOnlyCollectionPage<MeteringLabelRule>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
