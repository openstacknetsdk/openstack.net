using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    public class VolumeType : StringEnumeration
    {
        /// <summary />
        protected VolumeType()
        {
        }

        /// <summary />
        public VolumeType(string displayName)
            : base(displayName)
        {
        }

        /// <summary />
        public static readonly VolumeType Blank = new VolumeType("blank");

        /// <summary />
        public static readonly VolumeType Snapshot = new VolumeType("snapshot");

        /// <summary />
        public static readonly VolumeType Volume = new VolumeType("volume");

        /// <summary />
        public static readonly VolumeType Image = new VolumeType("image");

        /// <summary />
        public static readonly VolumeType Local = new VolumeType("local");
    }
}