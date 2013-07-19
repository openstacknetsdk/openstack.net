using System;

namespace net.openstack.Core
{
    public static class UserAgentGenerator
    {
        private static readonly Version _currentVersion = typeof(UserAgentGenerator).Assembly.GetName().Version;
        private static readonly string _userAgent = string.Format("openstack.net/{0}", _currentVersion);

        public static string UserAgent
        {
            get
            {
                return _userAgent;
            }
        }
    }
}
