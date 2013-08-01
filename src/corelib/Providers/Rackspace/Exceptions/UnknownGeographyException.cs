using System;
using System.Runtime.Serialization;
using System.Security;

namespace net.openstack.Providers.Rackspace.Exceptions
{
    [Serializable]
    public class UnknownGeographyException : Exception
    {
        public string Geo { get; private set; }

        public UnknownGeographyException(string geo) : base(string.Format("Unknown Geography: {0}", geo))
        {
            Geo = geo;
        }

        protected UnknownGeographyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Geo = (string)info.GetValue("Geo", typeof(string));
        }

        [SecurityCritical]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            base.GetObjectData(info, context);
            info.AddValue("Geo", Geo);
        }
    }
}
