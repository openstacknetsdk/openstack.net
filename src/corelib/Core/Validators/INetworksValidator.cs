namespace net.openstack.Core.Validators
{
    public interface INetworksValidator
    {
        void ValidateCidr(string cidr);    
    }
}
