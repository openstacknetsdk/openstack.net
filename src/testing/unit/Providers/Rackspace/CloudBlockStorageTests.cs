using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.openstack.Providers.Rackspace.Validators;

namespace OpenStackNet.Testing.Unit.Providers.Rackspace
{
    [TestClass]
    public class CloudBlockStorageTests
    {
        [TestMethod]
        public void Should_Not_Throw_Exception_When_Size_Is_In_Range()
        {
            const int size = 900;

            try
            {
                var cloudBlockStorageValidator = new CloudBlockStorageValidator();
                cloudBlockStorageValidator.ValidateVolumeSize(size);
            }
            catch (Exception ex)
            {

                Assert.Fail("Exception should not be thrown.");
            }
        }

        [TestMethod]
        public void Should_Throw_Exception_When_Size_Is_Less_Than_100()
        {
            const int size = 50;

            try
            {
                var cloudBlockStorageValidator = new CloudBlockStorageValidator();
                cloudBlockStorageValidator.ValidateVolumeSize(size);
                Assert.Fail("Expected was not thrown.");
            }
            catch (Exception ex)
            {

                Assert.AreEqual("ERROR: The volume size value must be between 100 and 1000", ex.Message);
            }
        }

        [TestMethod]
        public void Should_Throw_Exception_When_Size_Is_Greater_Than_1000()
        {
            const int size = 1050;

            try
            {
                var cloudBlockStorageValidator = new CloudBlockStorageValidator();
                cloudBlockStorageValidator.ValidateVolumeSize(size);
                Assert.Fail("Expected  was not thrown.");
            }
            catch (Exception ex)
            {

                Assert.AreEqual("ERROR: The volume size value must be between 100 and 1000", ex.Message);
            }
        }    
    }
}
