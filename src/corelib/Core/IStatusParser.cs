using net.openstack.Core.Domain;

namespace net.openstack.Core
{
    public interface IStatusParser
    {
        Status Parse(string value);
    }
}
