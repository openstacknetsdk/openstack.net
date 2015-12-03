using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    public class AddressType : StringEnumeration
    {
        /// <summary />
        protected AddressType(string displayName)
            : base(displayName)
        {
        }

        /// <summary />
        public static readonly AddressType Fixed = new AddressType("fixed");

        /// <summary />
        public static readonly AddressType Floating = new AddressType("floating");
    }
}