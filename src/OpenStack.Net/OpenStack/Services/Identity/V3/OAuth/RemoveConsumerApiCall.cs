namespace OpenStack.Services.Identity.V3.OAuth
{
    using OpenStack.Net;

    public class RemoveConsumerApiCall : DelegatingHttpApiCall<string>
    {
        public RemoveConsumerApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
