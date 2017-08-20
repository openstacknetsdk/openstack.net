using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenStack.Serialization;

namespace OpenStack.Images.v2
{

    /// <summary>
    ///Represent the image resource of <seealso cref="ImageService"/> 
    /// </summary>
    public class ImageOptions
    {
        /// <summary>
        ///The image identifier 
        /// </summary>
        [JsonProperty("id")]
        public Identifier Id;

        /// <summary>
        ///The name of the image.
        /// </summary>
        [JsonProperty("name")]
        public string Name;

        /// <summary>
        /// The image visibility. A valid value is public or private. Default is private.
        /// </summary>
        [JsonProperty("visibility")]
        public string Visibility;

        /// <summary>
        ///The ID of the owner, or tenant, of the image.
        ///The value might be null (JSON null data type).
        /// </summary>
        [JsonProperty("owner")]
        public string Owner;

        /// <summary>
        ///Image protection for deletion.A valid value is True or False.Default is False.
        /// </summary>
        [JsonProperty("protected")]
        public bool Protected;

        /// <summary>
        ///The size of the image data, in bytes.
        /// </summary>
        [JsonProperty("size")]
        public String Size;

        /// <summary>
        ///The image status 
        /// </summary>
        [JsonProperty("status")]
        public ImageStatus Status;

        /// <summary>
        /// The date and time when the resource was updated.
        /// </summary>
        [JsonProperty("updated_at")]
        public DateTime? UpdatedAt;

        /// <summary>
        ///The disk format of the image.
        /// </summary> 
        [JsonProperty("disk_format")]
        public string DiskFormat;

        /// <summary>
        ///The minimum disk size in GB that is required to boot the image. 
        /// </summary>
        [JsonProperty("min_disk")]
        public string MinDisk;

        /// <summary>
        ///The minimum amount of RAM in MB that is required to boot the image.
        /// </summary>
        [JsonProperty("min_ram")]
        public string MinRam;

        /// <summary>
        ///The date and time when the resource was created.
        /// </summary>
        [JsonProperty("created_at")]
        public DateTime? CreatedAt;

        /// <summary>
        ///The container format of the image.
        /// </summary>
        [JsonProperty("container_format")]
        public string ContainerFormat;

        /// <summary>
        ///The list of tag object
        /// </summary>
        [JsonProperty("tags")]
        public IList<string> Tags;

        /// <summary>
        ///Hash that is used over the image data 
        /// </summary>
        [JsonProperty("checksum")]
        public string Checksum;


        /// <summary>
        ///The URL for the virtual machine image. 
        /// </summary>
        [JsonProperty("self")]
        public string Self;

        /// <summary>
        ///The virtual size of the image. 
        /// </summary>
        [JsonProperty("virtual_size")]
        public string VirtualSize;

        /// <summary>
        ///The URL for the virtual machine image file. 
        /// </summary>
        [JsonProperty("file")]
        public string File;

        /// <summary>
        ///The URL for schema of the virtual machine image.
        /// </summary>
        [JsonProperty("schema")]
        public string Schema;

    }
}
