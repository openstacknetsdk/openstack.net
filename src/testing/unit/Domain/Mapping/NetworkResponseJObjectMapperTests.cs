using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using net.openstack.Core.Domain.Mapping;

namespace OpenStackNet.Testing.Unit.Domain.Mapping
{
    [TestClass]
    public class NetworkResponseJObjectMapperTests
    {
        [TestMethod]
        public void Should_Return_Null_When_Null_JObject_Is_Mapped()
        {
            var mapper = NetworkResponseJsonMapper.Default;

            JObject obj = null;
            var actual = mapper.Map(obj);

            Assert.IsNull(actual);
        }

        [TestMethod]
        public void Should_Return_Null_When_Null_String_Is_Mapped()
        {
            var mapper = NetworkResponseJsonMapper.Default;

            string obj = null;
            var actual = mapper.Map(obj);

            Assert.IsNull(actual);
        }

        [TestMethod]
        public void Should_Return_Null_When_Empty_String_Is_Mapped()
        {
            var mapper = NetworkResponseJsonMapper.Default;

            string obj = "";
            var actual = mapper.Map(obj);

            Assert.IsNull(actual);
        }

        [TestMethod]
        public void Should_Return_Null_When_Whitespace_String_Is_Mapped()
        {
            var mapper = NetworkResponseJsonMapper.Default;

            string obj = "   ";
            var actual = mapper.Map(obj);

            Assert.IsNull(actual);
        }

        [TestMethod]
        public void Should_Return_Network_With_No_Addresses()
        {
            var mapper = NetworkResponseJsonMapper.Default;

            string obj = "{\"public\": []}";
            var actual = mapper.Map(obj);

            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.Addresses.Any());
        }

        [TestMethod]
        public void Should_Return_Network_With_Single_Addresses()
        {
            var mapper = NetworkResponseJsonMapper.Default;

            string obj = "{\"public\": [{\"version\": 4, \"addr\": \"166.78.156.150\"}]}";
            var actual = mapper.Map(obj);

            Assert.IsNotNull(actual);
            Assert.AreEqual(actual.Addresses.Count(), 1);
        }

        [TestMethod]
        public void Should_Return_Network_With_2_Addresseses()
        {
            var mapper = NetworkResponseJsonMapper.Default;

            string obj = "{\"public\": [{\"version\": 4, \"addr\": \"166.78.156.150\"}, {\"version\": 6, \"addr\": \"2001:4800:7812:0514:95e4:7f4d:ff04:d1eb\"}]}";
            var actual = mapper.Map(obj);

            Assert.IsNotNull(actual);
            Assert.AreEqual(actual.Addresses.Count(), 2);
        }

        [TestMethod]
        public void Should_Return_Network_With_Both_v4_And_v6_Addresseses()
        {
            var mapper = NetworkResponseJsonMapper.Default;

            string obj = "{\"public\": [{\"version\": 4, \"addr\": \"166.78.156.150\"}, {\"version\": 6, \"addr\": \"2001:4800:7812:0514:95e4:7f4d:ff04:d1eb\"}]}";
            var actual = mapper.Map(obj);

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Addresses.SingleOrDefault(a => a.Version == "4"));
            Assert.IsNotNull(actual.Addresses.SingleOrDefault(a => a.Version == "6"));
        }
    }
}
