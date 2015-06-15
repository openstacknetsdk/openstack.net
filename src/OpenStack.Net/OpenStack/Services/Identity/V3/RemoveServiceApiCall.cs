namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class RemoveServiceApiCall : DelegatingHttpApiCall<string>
    {
        public RemoveServiceApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
