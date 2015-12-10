using System;
using System.Extensions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    [JsonConverterWithConstructor(typeof(RootWrapperConverter), "image")]
    public class Image : ImageReference
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Image"/> class.
        /// </summary>
        public Image()
        {
            Metadata = new ImageMetadata();
        }

        /// <summary />
        public override Identifier Id
        {
            get { return base.Id; }
            set
            {
                base.Id = value;

                // Since Metadata has nothing in it's json that indicates what image it is associated with, we need to manually remember it
                if (Metadata != null)
                    Metadata.ImageId = value;
            }
        }

        /// <summary />
        [JsonProperty("created")]
        public DateTimeOffset Created { get; set; }

        /// <summary />
        [JsonProperty("updated")]
        public DateTimeOffset LastModified { get; set; }

        /// <summary />
        [JsonProperty("minDisk")]
        public int MinimumDiskSize { get; set; }

        /// <summary />
        [JsonProperty("minRam")]
        public int MinimumMemorySize { get; set; }

        /// <summary />
        [JsonProperty("OS-EXT-IMG-SIZE:size")]
        public int? Size { get; set; }

        /// <summary />
        [JsonProperty("progress")]
        public int Progress { get; set; }

        /// <summary />
        [JsonProperty("status")]
        public ImageStatus Status { get; set; }

        /// <summary />
        [JsonProperty("server")]
        public ServerReference Server { get; set; }

        /// <summary />
        [JsonProperty("metadata")]
        public ImageMetadata Metadata { get; set; }

        /// <summary />
        [JsonIgnore]
        public ImageType Type
        {
            get
            {
                string type;
                if (Metadata != null && Metadata.TryGetValue("image_type", out type))
                    return StringEnumeration.FromDisplayName<ImageType>(type);

                return ImageType.Base;
            }
        }

        /// <inheritdoc cref="ComputeApiBuilder.WaitUntilImageIsActiveAsync" />
        /// <exception cref="InvalidOperationException">When this instance was not constructed by the <see cref="ComputeService"/>, as it is missing the appropriate internal state to execute service calls.</exception>
        public async Task WaitUntilActiveAsync(TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var owner = this.TryGetOwner<ComputeApiBuilder>();
            var result = await owner.WaitUntilImageIsActiveAsync(Id, refreshDelay, timeout, progress, cancellationToken).ConfigureAwait(false);
            result.CopyProperties(this);
        }

        /// <inheritdoc cref="ComputeApiBuilder.WaitUntilServerIsActiveAsync" />
        /// <exception cref="InvalidOperationException">When this instance was not constructed by the <see cref="ComputeService"/>, as it is missing the appropriate internal state to execute service calls.</exception>
        public override async Task DeleteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.DeleteAsync(cancellationToken).ConfigureAwait(false);
            Status = ImageStatus.Unknown;
        }

        /// <inheritdoc cref="ComputeApiBuilder.WaitUntilImageIsDeletedAsync" />
        /// <exception cref="InvalidOperationException">When this instance was not constructed by the <see cref="ComputeService"/>, as it is missing the appropriate internal state to execute service calls.</exception>
        public override async Task WaitUntilDeletedAsync(TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.WaitUntilDeletedAsync(refreshDelay, timeout, progress, cancellationToken).ConfigureAwait(false);
            Status = ImageStatus.Deleted;
        }
    }
}
