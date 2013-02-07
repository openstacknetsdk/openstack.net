using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.openstack.Core.Domain
{
    public class ServerState
    {
        public static string ACTIVE { get { return "ACTIVE"; } }

        public static string BUILD { get { return "BUILD"; } }

        public static string DELETED { get { return "DELETED"; } }

        public static string ERROR { get { return "ERROR"; } }

        public static string HARD_REBOOT { get { return "HARD_REBOOT"; } }

        public static string MIGRATING { get { return "MIGRATING"; } }

        public static string PASSWORD { get { return "PASSWORD"; } }

        public static string REBOOT { get { return "REBOOT"; } }

        public static string REBUILD { get { return "REBUILD"; } }

        public static string RESCUE { get { return "RESCUE"; } }

        public static string RESIZE { get { return "RESIZE"; } }

        public static string REVERT_RESIZE { get { return "REVERT_RESIZE"; } }

        public static string SUSPENDED { get { return "SUSPENDED"; } }

        public static string UNKNOWN { get { return "UNKNOWN"; } }

        public static string VERIFY_RESIZE { get { return "VERIFY_RESIZE"; } }
    }

    public class ImageState : ServerState{}
}
