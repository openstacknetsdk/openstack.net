using System;
using System.Runtime.Serialization;
using net.openstack.Core.Providers;

namespace net.openstack.Core.Domain
{
    [DataContract]
    public class SimpleServerImage : ProviderStateBase<IComputeProvider>
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public Link[] Links { get; set; }

        [DataMember]
        public string Name { get; set; }


        /// <summary>
        /// Waits for the image to enter the ACTIVE state
        /// </summary>
        /// <param name="refreshCount">Number of times to check the images status</param>
        /// <param name="refreshDelayInMS">The number of milliseconds to wait each time before requesting the status for the image.</param>
        /// <param name="progressUpdatedCallback">A callback delegate to execute each time the <see cref="SimpleServer"/>s Progress value increases.</param>
        public void WaitForActive(int refreshCount = 600, int refreshDelayInMS = 2400, Action<int> progressUpdatedCallback = null)
        {
            var details = Provider.WaitForImageActive(Id, refreshCount, refreshDelayInMS, progressUpdatedCallback, Region);
            UpdateThis(details);
        }

        /// <summary>
        /// Waits for the image to enter the DELETED state
        /// </summary>
        /// <param name="refreshCount">Number of times to check the images status</param>
        /// <param name="refreshDelayInMS">The number of milliseconds to wait each time before requesting the status for the image.</param>
        /// <param name="progressUpdatedCallback">A callback delegate to execute each time the <see cref="SimpleServer"/>s Progress value increases.</param>
        public void WaitForDelete(int refreshCount = 600, int refreshDelayInMS = 2400, Action<int> progressUpdatedCallback = null)
        {
            Provider.WaitForImageDeleted(Id, refreshCount, refreshDelayInMS, progressUpdatedCallback, Region);
        }


        /// <summary>
        /// Waits for the image to enter a particular <see cref="ImageState"/>
        /// </summary>
        /// <param name="expectedState">The expected <see cref="ImageState"/></param>
        /// <param name="errorStates">A list of <see cref="ImageState"/>s in which to throw an exception if the server enters. </param>
        /// <param name="refreshCount">Number of times to check the images status</param>
        /// <param name="refreshDelayInMS">The number of milliseconds to wait each time before requesting the status for the image.</param>
        /// <param name="progressUpdatedCallback">A callback delegate to execute each time the <see cref="SimpleServer"/>s Progress value increases.</param>
        public void WaitForState(string expectedState, string[] errorStates, int refreshCount = 600, int refreshDelayInMS = 2400, Action<int> progressUpdatedCallback = null)
        {
            var details = Provider.WaitForImageState(Id, expectedState, errorStates, refreshCount, refreshDelayInMS, progressUpdatedCallback, Region);
            UpdateThis(details);
        }

        /// <summary>
        /// Waits for the image to enter a particular <see cref="ImageState"/>
        /// </summary>
        /// <param name="expectedStates">The set expected <see cref="ImageState"/>s</param>
        /// <param name="errorStates">A list of <see cref="ImageState"/>s in which to throw an exception if the server enters. </param>
        /// <param name="refreshCount">Number of times to check the images status</param>
        /// <param name="refreshDelayInMS">The number of milliseconds to wait each time before requesting the status for the image.</param>
        /// <param name="progressUpdatedCallback">A callback delegate to execute each time the <see cref="SimpleServer"/>s Progress value increases.</param>
        public void WaitForState(string[] expectedStates, string[] errorStates, int refreshCount = 600, int refreshDelayInMS = 2400, Action<int> progressUpdatedCallback = null)
        {
            var details = Provider.WaitForImageState(Id, expectedStates, errorStates, refreshCount, refreshDelayInMS, progressUpdatedCallback, Region);
            UpdateThis(details);
        }

        protected virtual void UpdateThis(SimpleServerImage serverImage)
        {
            if (serverImage == null)
                return;

            Id = serverImage.Id;
            Links = serverImage.Links;
            Name = serverImage.Name;
        }

        /// <summary>
        /// Marks the image for deletion/>
        /// </summary>
        /// <returns><c>bool</c> indicating if the action was successful</returns>
        public bool Delete()
        {
            return Provider.DeleteImage(Id, Region);
        }
    }
}