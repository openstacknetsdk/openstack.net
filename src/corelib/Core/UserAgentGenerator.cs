using System;

namespace net.openstack.Core
{
    /// <summary>
    /// Generates the User-Agent value which identifies this SDK in REST requests.
    /// </summary>
    public static class UserAgentGenerator
    {
        private static readonly Version _currentVersion = typeof(UserAgentGenerator).Assembly.GetName().Version;
        private static readonly string _userAgent = string.Format("openstack.net/{0}", _currentVersion);

        /// <summary>
        /// Gets the User-Agent value for this SDK.
        /// </summary>
        public static string UserAgent
        {
            get
            {
                return _userAgent;
            }
        }
    }
}
