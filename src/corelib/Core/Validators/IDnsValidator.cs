namespace net.openstack.Core.Validators
{
    public interface IDnsValidator
    {
        void ValidateRecordType(string recordType);
        void ValidateTTL(int ttl);
    }
}