namespace OpenStack.Services.Networking.V2.LoadBalancer
{
    using System;
    using System.Net.Http;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;
    using OpenStack.ObjectModel.Converters;
    using OpenStack.Services.Identity;

    [JsonObject(MemberSerialization.OptIn)]
    public class HealthMonitorData : ExtensibleJsonObject
    {
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ProbeType _type;

        [JsonProperty("delay", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private int? _delay;

        [JsonProperty("timeout", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private int? _timeout;

        [JsonProperty("max_retries", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private int? _maxRetries;

        [JsonProperty("http_method", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(HttpMethodConverter))]
        private HttpMethod _httpMethod;

        [JsonProperty("url_path", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _urlPath;

        [JsonProperty("expected_codes", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _expectedCodes;

        [JsonProperty("admin_state_up", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private bool? _adminStateUp;

        [JsonProperty("tenant_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ProjectId _projectId;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthMonitorData"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected HealthMonitorData()
        {
        }

        public HealthMonitorData(ProbeType type, TimeSpan? delay, TimeSpan? timeout, int? maxRetries, HttpMethod httpMethod)
            : this(type, delay, timeout, maxRetries, httpMethod, null, null, null, null)
        {
        }

        public HealthMonitorData(ProbeType type, TimeSpan? delay, TimeSpan? timeout, int? maxRetries, HttpMethod httpMethod, Uri urlPath, string expectedCodes, bool? adminStateUp, ProjectId projectId)
        {
            _type = type;
            _delay = delay.HasValue ? (int?)delay.Value.TotalSeconds : null;
            _timeout = timeout.HasValue ? (int?)timeout.Value.TotalSeconds : null;
            _maxRetries = maxRetries;
            _httpMethod = httpMethod;
            _urlPath = urlPath.OriginalString;
            _expectedCodes = expectedCodes;
            _adminStateUp = adminStateUp;
            _projectId = projectId;
        }

        public ProbeType Type
        {
            get
            {
                return _type;
            }
        }

        public TimeSpan? Delay
        {
            get
            {
                if (_delay == null)
                    return null;

                return TimeSpan.FromSeconds(_delay.Value);
            }
        }

        public TimeSpan? Timeout
        {
            get
            {
                if (_timeout == null)
                    return null;

                return TimeSpan.FromSeconds(_timeout.Value);
            }
        }

        public int? MaxRetries
        {
            get
            {
                return _maxRetries;
            }
        }

        public HttpMethod HttpMethod
        {
            get
            {
                return _httpMethod;
            }
        }

        public Uri UrlPath
        {
            get
            {
                if (_urlPath == null)
                    return null;

                return new Uri(_urlPath, UriKind.Relative);
            }
        }

        public string ExpectedCodes
        {
            get
            {
                return _expectedCodes;
            }
        }

        public bool? AdminStateUp
        {
            get
            {
                return _adminStateUp;
            }
        }

        public ProjectId ProjectId
        {
            get
            {
                return _projectId;
            }
        }
    }
}
