namespace net.openstack.Core.Validators
{
    public interface IObjectStorageValidator
    {
        void ValidateContainerName(string containerName);
        void ValidateObjectName(string objectName);
    }
}