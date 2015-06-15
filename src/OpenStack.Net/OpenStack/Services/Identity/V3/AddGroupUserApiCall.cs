namespace OpenStack.Services.Identity.V3
{
    using OpenStack.Net;

    public class AddGroupUserApiCall : DelegatingHttpApiCall<string>
    {
        public AddGroupUserApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
