using System;
using System.Diagnostics;
using System.Reflection;

namespace net.openstack.Core
{
    /// <summary>
    /// Generates the User-Agent value which identifies this SDK in REST requests.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    public static class UserAgentGenerator
    {
        /// <summary>
        /// Gets the User-Agent value for this SDK.
        /// </summary>
        public static readonly string UserAgent = GetUserAgent();

        private static string GetUserAgent()
        {
            return string.Format("openstack.net/{0}", GetVersion());
        }

        private static string GetVersion()
        {
            Assembly sdkAssembly = typeof(UserAgentGenerator).Assembly;
            try
            {
                var fileVersionInfo = FileVersionInfo.GetVersionInfo(sdkAssembly.Location);
                return fileVersionInfo.FileVersion;
            }
            catch
            {
                return sdkAssembly.GetName().Version.ToString();
            }
        }
    }
}
