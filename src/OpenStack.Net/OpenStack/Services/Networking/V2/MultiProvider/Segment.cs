namespace OpenStack.Services.Networking.V2.MultiProvider
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;
    using NetworkType = OpenStack.Services.Networking.V2.Provider.NetworkType;
    using SegmentationId = OpenStack.Services.Networking.V2.Provider.SegmentationId;

    [JsonObject(MemberSerialization.OptIn)]
    public class Segment : ExtensibleJsonObject
    {
        [JsonProperty("provider:physical_network", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _physicalNetwork;

        [JsonProperty("provider:network_type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private NetworkType _networkType;

        [JsonProperty("provider:segmentation_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private SegmentationId _segmentationId;

        /// <summary>
        /// Initializes a new instance of the <see cref="Segment"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected Segment()
        {
        }

        public Segment(string physicalNetwork, NetworkType networkType, SegmentationId segmentationId)
        {
            _physicalNetwork = physicalNetwork;
            _networkType = networkType;
            _segmentationId = segmentationId;
        }

        public string PhysicalNetwork
        {
            get
            {
                return _physicalNetwork;
            }
        }

        public NetworkType NetworkType
        {
            get
            {
                return _networkType;
            }
        }

        public SegmentationId SegmentationId
        {
            get
            {
                return _segmentationId;
            }
        }
    }
}
