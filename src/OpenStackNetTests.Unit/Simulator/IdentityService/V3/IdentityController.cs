namespace OpenStackNetTests.Unit.Simulator.IdentityService.V3
{
    using System;
    using OpenStack.Services.Identity;

    public class IdentityController : BaseIdentityController
    {
        private static readonly string _username = "simulated_user";
        private static readonly string _password = "simulated_password";
        private static readonly string _tenantName = "simulated_tenant";
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// This value was generated specifically for use in this simulator.
        /// </remarks>
        private static readonly string _tenantId = "{BC5A63CD-F5B3-4D8F-AF4A-15FECE463D4D}";
        private static readonly string _userFullName = _username;
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// This value was generated specifically for use in this simulator.
        /// </remarks>
        private static readonly string _userId = "{97DD18E0-4763-4286-93B1-17B784FE8465}";

        /// <summary>
        /// This is used for synchronizing access to the authentication fields below.
        /// </summary>
        private static readonly object _lock = new object();

        private static TokenId _tokenId;
        private static DateTimeOffset? _tokenCreated;
        private static DateTimeOffset? _tokenExpires;

        internal static TokenId TokenId
        {
            get
            {
                return _tokenId;
            }
        }

        internal static DateTimeOffset? TokenExpires
        {
            get
            {
                return _tokenExpires;
            }
        }
    }
}
