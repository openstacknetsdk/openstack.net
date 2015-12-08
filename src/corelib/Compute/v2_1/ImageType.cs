using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary>
    /// The server image type
    /// </summary>
    public class ImageType : StringEnumeration
    {
        /// <summary />
        protected ImageType(string displayName) : base(displayName)
        { }

        /// <summary>
        /// Base Image
        /// </summary>
        public static ImageType Base = new ImageType("base");

        /// <summary>
        /// Server Backup
        /// </summary>
        public static ImageType Backup = new ImageType("backup");

        /// <summary>
        /// Server Snapshot
        /// </summary>
        public static ImageType Snapshot = new ImageType("snapshot");
    }
}