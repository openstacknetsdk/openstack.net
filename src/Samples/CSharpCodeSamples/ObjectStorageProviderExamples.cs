namespace CSharpCodeSamples
{
    using System;
    using System.Collections.Generic;
    using net.openstack.Core.Domain;
    using net.openstack.Core.Providers;

    public class ObjectStorageProviderExamples
    {
        #region ListObjectsInContainer
        public void ListObjects(IObjectStorageProvider provider, string containerName)
        {
            Console.WriteLine("Objects in container {0}", containerName);
            foreach (ContainerObject containerObject in ListAllObjects(provider, containerName))
                Console.WriteLine("    {0}", containerObject.Name);
        }

        private static IEnumerable<ContainerObject> ListAllObjects(
            IObjectStorageProvider provider,
            string containerName,
            int? blockSize = null,
            string prefix = null,
            string region = null,
            bool useInternalUrl = false,
            CloudIdentity identity = null)
        {
            if (blockSize <= 0)
                throw new ArgumentOutOfRangeException("blockSize");

            ContainerObject lastContainerObject = null;

            do
            {
                string marker = lastContainerObject != null ? lastContainerObject.Name : null;
                IEnumerable<ContainerObject> containerObjects =
                    provider.ListObjects(containerName, blockSize, marker, null, prefix, region, useInternalUrl, identity);
                lastContainerObject = null;
                foreach (ContainerObject containerObject in containerObjects)
                {
                    lastContainerObject = containerObject;
                    yield return containerObject;
                }
            } while (lastContainerObject != null);
        }
        #endregion ListObjectsInContainer
    }
}
