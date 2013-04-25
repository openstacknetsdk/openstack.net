using System.Runtime.Serialization;

namespace net.openstack.Core.Domain
{
    [DataContract]
    public class RateField
    {
        [DataMember(Name = "traceroute")] 
        public TracerouteField Traceroute;

        [DataMember(Name = "test_check")] 
        public TestCheckField TestCheck;

        [DataMember(Name = "test_alarm")] 
        public TestAlarmField TestAlarm;

        [DataMember(Name = "global")] 
        public GlobalField Global;

        [DataMember(Name = "test_notification")] 
        public TestNotificationField TestNotification;
    }
}