namespace net.openstack.Providers.Rackspace.Objects.Monitoring
{
    using System;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// This is the base class for classes modeling the detailed configuration parameters
    /// of various types of checks.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class CheckDetails
    {
        /// <summary>
        /// Deserializes a JSON object to a <see cref="CheckDetails"/> instance of the proper type.
        /// </summary>
        /// <param name="checkTypeId">The check type ID.</param>
        /// <param name="obj">The JSON object representing the check details.</param>
        /// <returns>A <see cref="CheckDetails"/> object corresponding to the JSON object.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="checkTypeId"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="obj"/> is <c>null</c>.</para>
        /// </exception>
        public static CheckDetails FromJObject(CheckTypeId checkTypeId, JObject obj)
        {
            if (checkTypeId == null)
                throw new ArgumentNullException("checkTypeId");
            if (obj == null)
                throw new ArgumentNullException("obj");

            if (checkTypeId == CheckTypeId.RemoteDns)
                return obj.ToObject<DnsCheckDetails>();
            else if (checkTypeId == CheckTypeId.RemoteFtpBanner)
                return obj.ToObject<FtpBannerCheckDetails>();
            else if (checkTypeId == CheckTypeId.RemoteHttp)
                return obj.ToObject<HttpCheckDetails>();
            else if (checkTypeId == CheckTypeId.RemoteImapBanner)
                return obj.ToObject<ImapBannerCheckDetails>();
            else if (checkTypeId == CheckTypeId.RemoteMssqlBanner)
                return obj.ToObject<MssqlBannerCheckDetails>();
            else if (checkTypeId == CheckTypeId.RemoteMysqlBanner)
                return obj.ToObject<MysqlBannerCheckDetails>();
            else if (checkTypeId == CheckTypeId.RemotePing)
                return obj.ToObject<PingCheckDetails>();
            else if (checkTypeId == CheckTypeId.RemotePop3Banner)
                return obj.ToObject<Pop3CheckDetails>();
            else if (checkTypeId == CheckTypeId.RemotePostgresqlBanner)
                return obj.ToObject<PostgresqlBannerCheckDetails>();
            else if (checkTypeId == CheckTypeId.RemoteSmtpBanner)
                return obj.ToObject<SmtpBannerCheckDetails>();
            else if (checkTypeId == CheckTypeId.RemoteSmtp)
                return obj.ToObject<SmtpCheckDetails>();
            else if (checkTypeId == CheckTypeId.RemoteSsh)
                return obj.ToObject<SshCheckDetails>();
            else if (checkTypeId == CheckTypeId.RemoteTcp)
                return obj.ToObject<TcpCheckDetails>();
            else if (checkTypeId == CheckTypeId.RemoteTelnetBanner)
                return obj.ToObject<TelnetBannerCheckDetails>();
            else
                return obj.ToObject<GenericCheckDetails>();
        }

        /// <summary>
        /// Determines whether the current <see cref="CheckDetails"/> object is compatible
        /// with checks of a particular type.
        /// </summary>
        /// <param name="checkTypeId">The check type ID.</param>
        /// <returns><c>true</c> if the current <see cref="CheckDetails"/> object is compatible with <paramref name="checkTypeId"/>; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="checkTypeId"/> is <c>null</c>.</exception>
        protected internal abstract bool SupportsCheckType(CheckTypeId checkTypeId);
    }
}
