using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.openstack.Core.Domain
{
    public class CloudBlockStorageVolumeType
    {   
        public static string SSD { get { return "SSD"; } }
        public static string SATA { get { return "SATA"; } }
    }
}
