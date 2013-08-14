using System.Linq;
using System.Net;
using System.Net.Sockets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.openstack.Core.Domain.Mapping;
using Newtonsoft.Json.Linq;

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
            Assert.AreEqual(0, actual.Addresses.Length);
        }

        [TestMethod]
        public void Should_Return_Network_With_Single_Addresses()
        {
            var mapper = NetworkResponseJsonMapper.Default;

            string obj = "{\"public\": [{\"version\": 4, \"addr\": \"166.78.156.150\"}]}";
            var actual = mapper.Map(obj);

            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.Addresses.Length);
            CollectionAssert.AllItemsAreNotNull(actual.Addresses);
            Assert.AreEqual(AddressFamily.InterNetwork, actual.Addresses[0].AddressFamily);
            Assert.AreEqual(IPAddress.Parse("166.78.156.150"), actual.Addresses[0]);
        }

        [TestMethod]
        public void Should_Return_Network_With_2_Addresseses()
        {
            var mapper = NetworkResponseJsonMapper.Default;

            string obj = "{\"public\": [{\"version\": 4, \"addr\": \"166.78.156.150\"}, {\"version\": 6, \"addr\": \"2001:4800:7812:0514:95e4:7f4d:ff04:d1eb\"}]}";
            var actual = mapper.Map(obj);

            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Addresses.Length);
            CollectionAssert.AllItemsAreNotNull(actual.Addresses);
            Assert.AreEqual(AddressFamily.InterNetwork, actual.Addresses[0].AddressFamily);
            Assert.AreEqual(IPAddress.Parse("166.78.156.150"), actual.Addresses[0]);
            Assert.AreEqual(AddressFamily.InterNetworkV6, actual.Addresses[1].AddressFamily);
            Assert.AreEqual(IPAddress.Parse("2001:4800:7812:0514:95e4:7f4d:ff04:d1eb"), actual.Addresses[1]);
        }

        [TestMethod]
        public void Should_Return_Network_With_Both_v4_And_v6_Addresseses()
        {
            var mapper = NetworkResponseJsonMapper.Default;

            string obj = "{\"public\": [{\"version\": 4, \"addr\": \"166.78.156.150\"}, {\"version\": 6, \"addr\": \"2001:4800:7812:0514:95e4:7f4d:ff04:d1eb\"}]}";
            var actual = mapper.Map(obj);

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Addresses.SingleOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork));
            Assert.IsNotNull(actual.Addresses.SingleOrDefault(a => a.AddressFamily == AddressFamily.InterNetworkV6));
        }
    }
}
