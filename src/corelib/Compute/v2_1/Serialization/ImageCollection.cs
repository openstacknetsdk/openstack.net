using System.Collections.Generic;
using Newtonsoft.Json;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1.Serialization
{
    /// <summary>
    /// Represents a collection of image resources of the <see cref="ComputeService"/>.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    public class ImageCollection<TPage, TItem> : Page<TPage, TItem, PageLink>
        where TPage : ImageCollection<TPage, TItem>
        where TItem : IServiceResource
    {
        /// <summary>
        /// The requested images.
        /// </summary>
        [JsonProperty("images")]
        protected IList<TItem> Flavors => Items;

        /// <summary>
        /// The paging navigation links.
        /// </summary>
        [JsonProperty("images_links")]
        protected IList<PageLink> ServerLinks => Links;
    }

    /// <summary>
    /// Represents a collection of references to image resources of the <see cref="ComputeService"/>.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    public class ImageReferenceCollection : ImageCollection<ImageReferenceCollection, ImageReference>
    { }

    /// <inheritdoc cref="FlavorCollection{T}" />
    public class ImageCollection : ImageCollection<ImageCollection, Image>
    { }
}