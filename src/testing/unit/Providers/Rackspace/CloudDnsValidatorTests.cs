using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using net.openstack.Core;
using net.openstack.Core.Exceptions;
using net.openstack.Core.Validators;
using net.openstack.Providers.Rackspace.Validators;

namespace OpenStackNet.Testing.Unit.Providers.Rackspace
{
    [TestClass]
    public class CloudDnsValidatorTests
    {
        [TestMethod]
        public void Should_Pass_When_A_RecordType()
        {
            var cloudDnsValidator = new CloudDnsValidator();
            cloudDnsValidator.ValidateRecordType("A");
        }

        [TestMethod]
        public void Should_Pass_When_a_RecordType()
        {
            var cloudDnsValidator = new CloudDnsValidator();
            cloudDnsValidator.ValidateRecordType("a");
        }

        [TestMethod]
        public void Should_Pass_When_CNAME_RecordType()
        {
            var cloudDnsValidator = new CloudDnsValidator();
            cloudDnsValidator.ValidateRecordType("CNAME");
        }

        [TestMethod]
        public void Should_Pass_When_MX_RecordType()
        {
            var cloudDnsValidator = new CloudDnsValidator();
            cloudDnsValidator.ValidateRecordType("MX");
        }

        [TestMethod]
        public void Should_Pass_When_NS_RecordType()
        {
            var cloudDnsValidator = new CloudDnsValidator();
            cloudDnsValidator.ValidateRecordType("NS");
        }

        [TestMethod]
        public void Should_Pass_When_SRV_RecordType()
        {
            var cloudDnsValidator = new CloudDnsValidator();
            cloudDnsValidator.ValidateRecordType("SRV");
        }

        [TestMethod]
        public void Should_Pass_When_TXT_RecordType()
        {
            var cloudDnsValidator = new CloudDnsValidator();
            cloudDnsValidator.ValidateRecordType("TXT");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidArgumentException))]
        public void Should_Fail_When_Unknown_RecordType()
        {
            var cloudDnsValidator = new CloudDnsValidator();
            cloudDnsValidator.ValidateRecordType("junk");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_Fail_When_Empty_RecordType()
        {
            var cloudDnsValidator = new CloudDnsValidator();
            cloudDnsValidator.ValidateRecordType(null);
        }

        [TestMethod]
        [ExpectedException(typeof(TTLLengthException))]
        public void Should_Fail_When_TTL_Less_Than_300()
        {
            var cloudDnsValidator = new CloudDnsValidator();
            cloudDnsValidator.ValidateTTL(299);
        }

        [TestMethod]
        public void Should_Pass_When_TTL_Greater_Than_300()
        {
            var cloudDnsValidator = new CloudDnsValidator();
            cloudDnsValidator.ValidateTTL(300);
        }
    }
}
