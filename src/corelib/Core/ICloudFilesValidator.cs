namespace net.openstack.Core
{
    public interface ICloudFilesValidator
    {
        void ValidateContainerName(string containerName);
        void ValidateObjectName(string objectName);
    }
}