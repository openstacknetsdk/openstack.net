namespace OpenStack.Services.Identity.V3.OAuth
{
    using OpenStack.Net;

    public class AddConsumerApiCall : DelegatingHttpApiCall<ConsumerResponse>
    {
        public AddConsumerApiCall(IHttpApiCall<ConsumerResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
