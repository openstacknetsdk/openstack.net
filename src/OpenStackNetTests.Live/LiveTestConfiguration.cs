namespace OpenStackNetTests.Live
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;
    using File = System.IO.File;
    using FileNotFoundException = System.IO.FileNotFoundException;
    using Path = System.IO.Path;

    [JsonObject(MemberSerialization.OptIn)]
    internal class LiveTestConfiguration : ExtensibleJsonObject
    {
#pragma warning disable 649 // Field 'fieldName' is never assigned to, and will always have its default value {value}
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _name;

        [JsonProperty("selectedCredentials", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _selectedCredentials;

        [JsonProperty("credentials", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private TestCredentials[] _credentials;
#pragma warning restore 649

        /// <summary>
        /// Initializes a new instance of the <see cref="LiveTestConfiguration"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected LiveTestConfiguration()
        {
        }

        public static LiveTestConfiguration LoadDefaultConfiguration()
        {
            string path = Path.GetFullPath(@"..\..\..\..\..\LiveTestConfiguration.json");
            return Load(path);
        }

        public static LiveTestConfiguration Load(string location)
        {
            if (location == null)
                throw new ArgumentNullException("location");
            if (string.IsNullOrEmpty(location))
                throw new ArgumentException("location cannot be empty");
            if (!Path.IsPathRooted(location))
                throw new ArgumentException("location must be an absolute path", "location");
            if (!File.Exists(location))
                throw new FileNotFoundException(location);

            return JsonConvert.DeserializeObject<LiveTestConfiguration>(File.ReadAllText(location));
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string SelectedCredentials
        {
            get
            {
                return _selectedCredentials;
            }
        }

        public ReadOnlyCollection<TestCredentials> Credentials
        {
            get
            {
                if (_credentials == null)
                    return null;

                return new ReadOnlyCollection<TestCredentials>(_credentials);
            }
        }

        public TestCredentials TryGetSelectedCredentials()
        {
            return TryGetCredentials(SelectedCredentials);
        }

        public TestCredentials TryGetCredentials(string name)
        {
            if (_credentials == null)
                return null;

            if (name == null)
                return null;

            return Credentials.SingleOrDefault(i => string.Equals(name, i.Name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
