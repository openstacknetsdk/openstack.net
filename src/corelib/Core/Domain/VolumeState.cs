namespace net.openstack.Core.Domain
{
    public class VolumeState
    {
        public static string CREATING { get { return "CREATING"; } }

        public static string AVAILABLE { get { return "AVAILABLE"; } }

        public static string ATTACHING { get { return "ATTACHING"; } }

        public static string IN_USE { get { return "IN-USE"; } }

        public static string DELETING { get { return "DELETING"; } }

        public static string ERROR { get { return "ERROR"; } }

        public static string ERROR_DELETING { get { return "ERROR_DELETING"; } }

    }

    public class SnapshotState : VolumeState { }
}
