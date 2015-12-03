using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    public class DiskConfiguration : StringEnumeration
    {
        /// <summary />
        protected DiskConfiguration(string displayName)
            : base(displayName)
        {
        }

        /// <summary />
        public static readonly DiskConfiguration Auto = new DiskConfiguration("AUTO");

        /// <summary />
        public static readonly DiskConfiguration Manual = new DiskConfiguration("MANUAL");
    }
}