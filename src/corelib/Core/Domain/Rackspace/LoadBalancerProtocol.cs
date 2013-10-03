using System.Collections.Generic;
using System;
using System.Collections.Concurrent;
using net.openstack.Core.Domain.Converters;
using Newtonsoft.Json;

namespace net.openstack.Core.Domain.Rackspace
{
    /// <summary>
    /// Represents the protocols of a Load Balancer.
    /// </summary>
    /// <remarks>
    /// This class functions as a strongly-typed enumeration of known load balancer protocols
    /// </remarks>
    /// <threadsafety static="true" instance="false"/>
    [JsonConverter(typeof (LoadBalancerProtocol.Converter))]
    public sealed class LoadBalancerProtocol : IEquatable<LoadBalancerProtocol>
    {
        private static readonly ConcurrentDictionary<string, LoadBalancerProtocol> _states =
            new ConcurrentDictionary<string, LoadBalancerProtocol>(StringComparer.OrdinalIgnoreCase);

        private static readonly Dictionary<string, int> _protocols = new Dictionary<string, int> 
        {
            {"DNS_TCP", 53},
            {"DNS_UDP",53},
            {"FTP", 21},
            {"HTTP", 80},
            {"HTTPS", 443},
            {"IMAPS", 993},
            {"IMAPv4", 143},
            {"LDAP", 389},
            {"LDAPS", 636},
            {"MYSQL", 3306},
            {"POP3", 110},
            {"POP3S", 995},
            {"SMTP", 25},
            {"TCP", 0},
            {"TCP_CLIENT_FIRST", 0},
            {"UDP", 0},
            {"UDP_STREAM", 0},
            {"SFTP", 22}
        };

        private static readonly LoadBalancerProtocol _dnsTCP = FromName("DNS_TCP");
        private static readonly LoadBalancerProtocol _dnsUDP = FromName("DNS_UDP");
        private static readonly LoadBalancerProtocol _ftp = FromName("FTP");
        private static readonly LoadBalancerProtocol _http = FromName("HTTP");
        private static readonly LoadBalancerProtocol _https = FromName("HTTPS");
        private static readonly LoadBalancerProtocol _imas = FromName("IMAPS");
        private static readonly LoadBalancerProtocol _imasv4 = FromName("IMAPv4");
        private static readonly LoadBalancerProtocol _ldap = FromName("LDAP");
        private static readonly LoadBalancerProtocol _ldaps = FromName("LDAPS");
        private static readonly LoadBalancerProtocol _mysql = FromName("MYSQL");
        private static readonly LoadBalancerProtocol _pop3 = FromName("POP3");
        private static readonly LoadBalancerProtocol _pop3s = FromName("POP3S");
        private static readonly LoadBalancerProtocol _smtp = FromName("SMTP");
        private static readonly LoadBalancerProtocol _tcp = FromName("TCP");
        private static readonly LoadBalancerProtocol _tcpClientFirst = FromName("TCP_CLIENT_FIRST");
        private static readonly LoadBalancerProtocol _udp = FromName("UDP");
        private static readonly LoadBalancerProtocol _udpStream = FromName("UDP_STREAM");
        private static readonly LoadBalancerProtocol _sftp = FromName("SFTP");

        private readonly string _name;
        private readonly int _port;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadBalancerProtocol"/> class with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        private LoadBalancerProtocol(string name, int port)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            _name = name;
            _port = port;
        }

        /// <summary>
        /// Gets the <see cref="LoadBalancerProtocol"/> instance with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        public static LoadBalancerProtocol FromName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            var port = _protocols[name];

            return _states.GetOrAdd(name, i => new LoadBalancerProtocol(i, port));
        }
        
        /// <summary>
        /// Gets an <see cref="LoadBalancerProtocol"/> representing the DNS_TCP protocol.
        /// </summary>
        public static LoadBalancerProtocol DNS_TCP
        {
            get { return _dnsTCP; }
        }

        /// <summary>
        /// Gets an <see cref="LoadBalancerProtocol"/> representing the DNS_UDP protocol.
        /// </summary>
        public static LoadBalancerProtocol DNS_UDP
        {
            get { return _dnsUDP; }
        }

        /// <summary>
        /// Gets an <see cref="LoadBalancerProtocol"/> representing the FTP protocol.
        /// </summary>
        public static LoadBalancerProtocol FTP
        {
            get { return _ftp; }
        }

        /// <summary>
        /// Gets an <see cref="LoadBalancerProtocol"/> representing the HTTP protocol.
        /// </summary>
        public static LoadBalancerProtocol HTTP
        {
            get { return _http; }
        }

        /// <summary>
        /// Gets an <see cref="LoadBalancerProtocol"/> representing the HTTPS protocol.
        /// </summary>
        public static LoadBalancerProtocol HTTPS
        {
            get { return _https; }
        }

        /// <summary>
        /// Gets an <see cref="LoadBalancerProtocol"/> representing the IMAPS protocol.
        /// </summary>
        public static LoadBalancerProtocol IMAPS
        {
            get { return _imas; }
        }

        /// <summary>
        /// Gets an <see cref="LoadBalancerProtocol"/> representing the IMAPv4 protocol.
        /// </summary>
        public static LoadBalancerProtocol IMAPv4
        {
            get { return _imasv4; }
        }

        /// <summary>
        /// Gets an <see cref="LoadBalancerProtocol"/> representing the LDAP protocol.
        /// </summary>
        public static LoadBalancerProtocol LDAP
        {
            get { return _ldap; }
        }

        /// <summary>
        /// Gets an <see cref="LoadBalancerProtocol"/> representing the LDAPS protocol.
        /// </summary>
        public static LoadBalancerProtocol LDAPS
        {
            get { return _ldaps; }
        }

        /// <summary>
        /// Gets an <see cref="LoadBalancerProtocol"/> representing the MySQL protocol.
        /// </summary>
        public static LoadBalancerProtocol MySQL
        {
            get { return _mysql; }
        }

        /// <summary>
        /// Gets an <see cref="LoadBalancerProtocol"/> representing the POP3 protocol.
        /// </summary>
        public static LoadBalancerProtocol POP3
        {
            get { return _pop3; }
        }

        /// <summary>
        /// Gets an <see cref="LoadBalancerProtocol"/> representing the POP3S protocol.
        /// </summary>
        public static LoadBalancerProtocol POP3S
        {
            get { return _pop3s; }
        }

        /// <summary>
        /// Gets an <see cref="LoadBalancerProtocol"/> representing the SMTP protocol.
        /// </summary>
        public static LoadBalancerProtocol SMTP
        {
            get { return _smtp; }
        }

        /// <summary>
        /// Gets an <see cref="LoadBalancerProtocol"/> representing the TCP protocol.
        /// </summary>
        public static LoadBalancerProtocol TCP
        {
            get { return _tcp; }
        }

        /// <summary>
        /// Gets an <see cref="LoadBalancerProtocol"/> representing the TCP_CLIENT_FIRST protocol.
        /// </summary>
        public static LoadBalancerProtocol TCP_Client_First
        {
            get { return _tcpClientFirst; }
        }

        /// <summary>
        /// Gets an <see cref="LoadBalancerProtocol"/> representing the UDP protocol.
        /// </summary>
        public static LoadBalancerProtocol UDP
        {
            get { return _udp; }
        }

        /// <summary>
        /// Gets an <see cref="LoadBalancerProtocol"/> representing the UDP_STREAM protocol.
        /// </summary>
        public static LoadBalancerProtocol UDP_Stream
        {
            get { return _udpStream; }
        }

        /// <summary>
        /// Gets an <see cref="LoadBalancerProtocol"/> representing the SFTP protocol.
        /// </summary>
        public static LoadBalancerProtocol SFTP
        {
            get { return _sftp; }
        }

        /// <summary>
        /// Gets the canonical name of the load balancer protocol.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets the port of the load balancer protocol.
        /// </summary>
        public int Port
        {
            get { return _port; }
        }

        /// <inheritdoc/>
        public bool Equals(LoadBalancerProtocol other)
        {
            return this == other;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="LoadBalancerProtocol"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        private sealed class Converter : SimpleStringJsonConverter<LoadBalancerProtocol>
        {
            /// <remarks>
            /// This method uses <see cref="Name"/> for serialization.
            /// </remarks>
            /// <inheritdoc/>
            protected override string ConvertToString(LoadBalancerProtocol obj)
            {
                return obj.Name;
            }

            /// <remarks>
            /// If <paramref name="str"/> is an empty string, this method returns <c>null</c>.
            /// Otherwise, this method uses <see cref="FromName"/> for deserialization.
            /// </remarks>
            /// <inheritdoc/>
            protected override LoadBalancerProtocol ConvertToObject(string str)
            {
                if (string.IsNullOrEmpty(str))
                    return null;

                return FromName(str);
            }
        }
    }
}