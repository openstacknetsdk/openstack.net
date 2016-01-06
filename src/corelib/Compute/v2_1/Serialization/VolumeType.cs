using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1.Serialization
{
    /// <summary />
    public class VolumeType<T> : StringEnumeration
        where T : VolumeType<T>, new()
    {
        /// <summary />
        public static readonly T Blank = new T{DisplayName = "blank"};

        /// <summary />
        public static readonly T Snapshot = new T{DisplayName = "snapshot"};

        /// <summary />
        public static readonly T Volume = new T{DisplayName = "volume"};

        /// <summary />
        public static readonly T Image = new T{DisplayName = "image"};

        /// <summary />
        public static readonly T Local = new T{DisplayName = "local"};
    }
}