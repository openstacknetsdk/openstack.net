namespace OpenStack.Services.Identity.V3.OAuth
{
    using OpenStack.Net;

    public class GetConsumerApiCall : DelegatingHttpApiCall<ConsumerResponse>
    {
        public GetConsumerApiCall(IHttpApiCall<ConsumerResponse> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
