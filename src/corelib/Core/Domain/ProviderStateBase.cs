using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.openstack.Core.Domain
{
    public abstract class ProviderStateBase<T>
    {
        internal T Provider { get; set; }

        internal string Region { get; set; }

        internal CloudIdentity Identity { get; set; }
    }
}
