using System.Runtime.Serialization;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects.Request
{
    [DataContract]
    internal class ServerRebootDetails
    {
        [DataMember(Name = "type")]
        private string _type;

        public RebootType Type
        {
            get
            {
                if (string.IsNullOrEmpty(_type))
                    return null;

                return RebootType.FromName(_type);
            }

            set
            {
                if (value == null)
                    _type = null;
                else
                    _type = value.Name;
            }
        }
    }
}