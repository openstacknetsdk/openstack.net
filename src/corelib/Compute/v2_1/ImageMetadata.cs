using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenStack.Compute.v2_1.Serialization;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    [JsonConverterWithConstructor(typeof (RootWrapperConverter), "metadata")]
    public class ImageMetadata : Dictionary<string, string>, IHaveExtraData, IChildResource
    {
        /// <summary />
        [JsonIgnore]
        protected ImageReference Image { get; set; }

        /// <summary />
        [JsonExtensionData]
        IDictionary<string, JToken> IHaveExtraData.Data { get; set; } = new Dictionary<string, JToken>();

        object IServiceResource.Owner { get; set; }

        /// <summary />
        protected internal void SetParent(ImageReference parent)
        {
            Image = parent;
        }

        void IChildResource.SetParent(string parentId)
        {
            SetParent(new ImageReference { Id = parentId});
        }

        void IChildResource.SetParent(object parent)
        {
            SetParent((ImageReference)parent);
        }

        /// <summary />
        protected void AssertParentIsSet([CallerMemberName]string callerName = "")
        {
            if (Image != null)
                return;

            throw new InvalidOperationException(string.Format($"{callerName} can only be used on instances which were constructed by the ComputeService. Use ComputeService.{callerName} instead."));
        }

        /// <summary />
        public async Task CreateAsync(string key, string value, CancellationToken cancellationToken = default(CancellationToken))
        {
            AssertParentIsSet();
            var compute = this.GetOwnerOrThrow<ComputeApi>();
            await compute.CreateImagMetadataAsync(Image.Id, key, value, cancellationToken);
            this[key] = value;
        }

        /// <summary />
        public async Task UpdateAsync(bool overwrite = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            AssertParentIsSet();
            var compute = this.GetOwnerOrThrow<ComputeApi>();
            var results = await compute.UpdateImageMetadataAsync<ImageMetadata>(Image.Id, this, overwrite, cancellationToken);
            Clear();
            foreach (var result in results)
            {
                Add(result.Key, result.Value);
            }
        }

        /// <summary />
        public async Task DeleteAsync(string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!ContainsKey(key))
                return;

            AssertParentIsSet();
            var compute = this.GetOwnerOrThrow<ComputeApi>();
            await compute.DeleteImageMetadataAsync(Image.Id, key, cancellationToken);
            Remove(key);
        }
    }
}