namespace net.openstack.Core
{
    public interface ICloudBlockStorageValidator
    {
        void ValidateVolumeSize(int size);
    }
}
