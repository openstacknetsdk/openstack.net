namespace net.openstack.Core.Domain
{
    public class Status
    {
        public int Code { get; private set; }

        public string Description { get; private set; }

        public Status(int code, string description)
        {
            Code = code;
            Description = description;
        }
    }
}
