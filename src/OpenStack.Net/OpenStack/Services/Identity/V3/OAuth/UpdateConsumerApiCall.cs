namespace OpenStack.Services.Identity.V3.OAuth
{
    using OpenStack.Net;

    public class UpdateConsumerApiCall : DelegatingHttpApiCall<ConsumerResponse>
    {
        public UpdateConsumerApiCall(IHttpApiCall<ConsumerResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
