namespace net.openstack.Core.Domain
{
    using System;
    using net.openstack.Core.Providers;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class SimpleServerImage : ProviderStateBase<IComputeProvider>
    {
        [JsonProperty]
        public string Id { get; private set; }

        [JsonProperty]
        public Link[] Links { get; private set; }

        [JsonProperty]
        public string Name { get; private set; }


        /// <summary>
        /// Waits for the image to enter the ACTIVE state
        /// </summary>
        /// <param name="refreshCount">Number of times to check the images status</param>
        /// <param name="refreshDelay">The time to wait each time before requesting the status for the image. If this value is <c>null</c>, the default is 2.4 seconds.</param>
        /// <param name="progressUpdatedCallback">A callback delegate to execute each time the <see cref="SimpleServer"/>s Progress value increases.</param>
        public void WaitForActive(int refreshCount = 600, TimeSpan? refreshDelay = null, Action<int> progressUpdatedCallback = null)
        {
            var details = Provider.WaitForImageActive(Id, refreshCount, refreshDelay, progressUpdatedCallback, Region, Identity);
            UpdateThis(details);
        }

        /// <summary>
        /// Waits for the image to enter the DELETED state
        /// </summary>
        /// <param name="refreshCount">Number of times to check the images status</param>
        /// <param name="refreshDelay">The time to wait each time before requesting the status for the image. If this value is <c>null</c>, the default is 2.4 seconds.</param>
        /// <param name="progressUpdatedCallback">A callback delegate to execute each time the <see cref="SimpleServer"/>s Progress value increases.</param>
        public void WaitForDelete(int refreshCount = 600, TimeSpan? refreshDelay = null, Action<int> progressUpdatedCallback = null)
        {
            Provider.WaitForImageDeleted(Id, refreshCount, refreshDelay, progressUpdatedCallback, Region, Identity);
        }


        /// <summary>
        /// Waits for the image to enter a particular <see cref="ImageState"/>
        /// </summary>
        /// <param name="expectedState">The expected <see cref="ImageState"/></param>
        /// <param name="errorStates">A list of <see cref="ImageState"/>s in which to throw an exception if the server enters. </param>
        /// <param name="refreshCount">Number of times to check the images status</param>
        /// <param name="refreshDelay">The time to wait each time before requesting the status for the image. If this value is <c>null</c>, the default is 2.4 seconds.</param>
        /// <param name="progressUpdatedCallback">A callback delegate to execute each time the <see cref="SimpleServer"/>s Progress value increases.</param>
        public void WaitForState(ImageState expectedState, ImageState[] errorStates, int refreshCount = 600, TimeSpan? refreshDelay = null, Action<int> progressUpdatedCallback = null)
        {
            var details = Provider.WaitForImageState(Id, expectedState, errorStates, refreshCount, refreshDelay, progressUpdatedCallback, Region, Identity);
            UpdateThis(details);
        }

        /// <summary>
        /// Waits for the image to enter a particular <see cref="ImageState"/>
        /// </summary>
        /// <param name="expectedStates">The set expected <see cref="ImageState"/>s</param>
        /// <param name="errorStates">A list of <see cref="ImageState"/>s in which to throw an exception if the server enters. </param>
        /// <param name="refreshCount">Number of times to check the images status</param>
        /// <param name="refreshDelay">The time to wait each time before requesting the status for the image. If this value is <c>null</c>, the default is 2.4 seconds.</param>
        /// <param name="progressUpdatedCallback">A callback delegate to execute each time the <see cref="SimpleServer"/>s Progress value increases.</param>
        public void WaitForState(ImageState[] expectedStates, ImageState[] errorStates, int refreshCount = 600, TimeSpan? refreshDelay = null, Action<int> progressUpdatedCallback = null)
        {
            var details = Provider.WaitForImageState(Id, expectedStates, errorStates, refreshCount, refreshDelay, progressUpdatedCallback, Region, Identity);
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
            return Provider.DeleteImage(Id, Region, Identity);
        }
    }
}
