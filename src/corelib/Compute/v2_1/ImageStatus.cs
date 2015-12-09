using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    public class ImageStatus : StringEnumeration
    {
        /// <summary />
        protected ImageStatus(string displayName) : base(displayName)
        { }

        /// <summary />
        public static readonly ImageStatus Active = new ImageStatus("ACTIVE");

        /// <summary />
        public static readonly ImageStatus Saving = new ImageStatus("SAVING");

        /// <summary />
        public static readonly ImageStatus Deleted = new ImageStatus("DELETED");

        /// <summary />
        public static readonly ImageStatus Error = new ImageStatus("ERROR");

        /// <summary />
        public static readonly ImageStatus Unknown = new ImageStatus("UNKNOWN");
    }
}