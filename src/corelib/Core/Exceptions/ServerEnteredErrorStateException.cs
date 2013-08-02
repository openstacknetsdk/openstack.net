using System;
using System.Runtime.Serialization;
using System.Security;

namespace net.openstack.Core.Exceptions
{
    [Serializable]
    public class ServerEnteredErrorStateException : Exception
    {
        public string Status { get; private set; }

        public ServerEnteredErrorStateException(string status)
            : base(string.Format("The server entered an error state: '{0}'", status))
        {
            Status = status;
        }

        protected ServerEnteredErrorStateException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Status = (string)info.GetValue("Status", typeof(string));
        }

        [SecurityCritical]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            base.GetObjectData(info, context);
            info.AddValue("Status", Status);
        }
    }
}
