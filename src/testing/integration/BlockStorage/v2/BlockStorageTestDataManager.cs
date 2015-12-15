using System;
using System.Collections.Generic;
using System.Linq;
using net.openstack.Core.Domain;
using net.openstack.Core.Providers;

namespace OpenStack.BlockStorage.v2
{
    public class BlockStorageTestDataManager : IDisposable
    {
        private readonly IBlockStorageProvider _storage;
        private readonly HashSet<object> _testData;
         
        public BlockStorageTestDataManager(IBlockStorageProvider storage)
        {
            _storage = storage;
            _testData = new HashSet<object>();
        }

        public void Register(IEnumerable<object> testItems)
        {
            foreach (var testItem in testItems)
            {
                Register(testItem);
            }
        }

        public void Register(object testItem)
        {
            _testData.Add(testItem);
        }

        public void Dispose()
        {
            var errors = new List<Exception>();
            try
            {
                DeleteVolumes(_testData.OfType<Volume>());
            }
            catch (AggregateException ex) { errors.AddRange(ex.InnerExceptions); }

            if (errors.Any())
                throw new AggregateException("Unable to remove all test data!", errors);
        }

        #region Volumes
        public Volume CreateVolume()
        {
            var volume = _storage.CreateVolume(1);
            Register(volume);
            return volume;
        }

        public void WaitUntilAttached(Identifier volumeId)
        {
            _storage.WaitForVolumeState(volumeId, VolumeState.InUse, new[] {VolumeState.Error});
        }

        public void DeleteVolumes(IEnumerable<Volume> volumes)
        {
            foreach (Volume volume in volumes)
            {
                _storage.DeleteVolume(volume.Id);
            }
        }
        #endregion
    }
}
