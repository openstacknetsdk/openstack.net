using Newtonsoft.Json.Linq;

namespace net.openstack.corelib
{
    public class CloudIdentity
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string APIKey { get; set; }

        public string AccountId { get; set; }

        public string Region { get; set; }

        public object BuildAuthRequestJson()
        {
            var request = new JObject();
            var impInfo = new JObject();
            var user = new JObject { { "username", Username }};
            if (string.IsNullOrWhiteSpace(Password))
            {
                user.Add("apiKey", APIKey);
                impInfo.Add("RAX-KSKEY:apiKeyCredentials", user);
            }
            else
            {
                user.Add("password", Password);
                impInfo.Add("passwordCredentials", user);
            }
            request.Add("auth", impInfo);

            return request;
        }
    }
}
