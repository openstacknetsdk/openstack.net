namespace net.openstack.Core.Validators
{
    public interface IBlockStorageValidator
    {
        void ValidateVolumeSize(int size);
    }
}
