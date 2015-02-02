namespace OpenStackNetTests.Unit
{
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenStack.Net;
    using OpenStackNetTests.Live;

    [TestClass]
    public class TestIPAddress
    {
        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public void TestIPAddressV4None()
        {
            Assert.AreEqual(AddressFamily.Ipv4, IPAddress.None.AddressFamily);
            Assert.AreEqual("255.255.255.255", IPAddress.None.ToString());
            Assert.AreEqual<System.Net.IPAddress>(System.Net.IPAddress.None, IPAddress.None);
            Assert.AreEqual<IPAddress>(IPAddress.None, System.Net.IPAddress.None);
        }

        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public void TestIPAddressV6None()
        {
            Assert.AreEqual(AddressFamily.Ipv6, IPAddress.IPv6None.AddressFamily);
            Assert.AreEqual("::", IPAddress.IPv6None.ToString());
            Assert.AreEqual<System.Net.IPAddress>(System.Net.IPAddress.IPv6None, IPAddress.IPv6None);
            Assert.AreEqual<IPAddress>(IPAddress.IPv6None, System.Net.IPAddress.IPv6None);
        }

        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public void TestParseIPv4()
        {
            string addressString = "10.35.90.255";
            CompareBehavior(addressString);
        }

        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public void TestParseIPv6()
        {
            string addressString = "::";
            Assert.AreEqual(IPAddress.IPv6None, IPAddress.Parse(addressString));
            CompareBehavior(addressString);

            // Additional cases from RFC 5952
            CompareBehavior("2001:db8:aaaa:bbbb:cccc:dddd:0:1");
            CompareBehavior("2001:db8::1");
            CompareBehavior("2001:db8::aaaa:0:0:1");
            CompareBehavior("2001:db8::1:0:0:1");

            // Additional edge cases
            CompareBehavior("2001:db8::");
            CompareBehavior("::1");
            CompareBehavior("::2001:db8", checkFrameworkString: false);
        }

        private void CompareBehavior(string addressString, bool checkSdkString = true, bool checkFrameworkString = true)
        {
            IPAddress address = IPAddress.Parse(addressString);
            if (checkSdkString)
                Assert.AreEqual(addressString, address.ToString());

            System.Net.IPAddress systemAddress = System.Net.IPAddress.Parse(addressString);
            if (checkFrameworkString)
                Assert.AreEqual(addressString, systemAddress.ToString());

            Assert.AreEqual<IPAddress>(address, systemAddress);
            Assert.AreEqual<System.Net.IPAddress>(systemAddress, address);

            Assert.AreEqual<AddressFamily>(address.AddressFamily, systemAddress.AddressFamily);
            Assert.IsTrue(address.GetAddressBytes().SequenceEqual(systemAddress.GetAddressBytes()));
        }
    }
}
