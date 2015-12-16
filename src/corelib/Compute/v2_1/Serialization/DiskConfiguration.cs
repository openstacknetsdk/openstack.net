using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1.Serialization
{
    /// <summary />
    public class DiskConfiguration<T> : StringEnumeration
        where T : DiskConfiguration<T>, new()
    {
        /// <summary />
        public static readonly T Auto = new T {DisplayName = "AUTO"};

        /// <summary />
        public static readonly T Manual = new T {DisplayName = "MANUAL"};
    }
}