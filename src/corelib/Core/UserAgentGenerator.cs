using System;
using System.Reflection;

namespace net.openstack.Core
{
    public class UserAgentGenerator
    {
        private static Version _currentVersion;

        public static string Generate()
        {
            if (_currentVersion == null)
                _currentVersion = Assembly.GetExecutingAssembly().GetName().Version;

            return string.Format("openstack.net/{0}", _currentVersion.ToString());
        }
    }
}
