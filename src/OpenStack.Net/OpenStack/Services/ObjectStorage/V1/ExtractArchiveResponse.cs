namespace OpenStack.Services.ObjectStorage.V1
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Collections.ObjectModel;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

#if PORTABLE
    using OpenStack.Collections;
#endif

    /// <summary>
    /// This class models the JSON response to an Extract Archive operation.
    /// </summary>
    /// <seealso cref="O:OpenStack.Services.ObjectStorage.V1.ExtractArchiveExtensions.ExtractArchiveAsync"/>
    /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/archive-auto-extract.html">Auto-extract archive files (OpenStack Object Storage API V1 Reference)</seealso>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonObject(MemberSerialization.OptIn)]
    public class ExtractArchiveResponse : ExtensibleJsonObject
    {
#pragma warning disable 649 // Field 'fieldName' is never assigned to, and will always have its default value {value}
        /// <summary>
        /// This is the backing field for the <see cref="CreatedFiles"/> property.
        /// </summary>
        [JsonProperty("Number Files Created")]
        private int? _createdFiles;

        /// <summary>
        /// This is the backing field for the <see cref="ResponseStatus"/> property.
        /// </summary>
        [JsonProperty("Response Status")]
        private string _responseStatus;

        /// <summary>
        /// This is the backing field for the <see cref="ResponseBody"/> property.
        /// </summary>
        [JsonProperty("Response Body")]
        private string _responseBody;

        /// <summary>
        /// This is the backing field for the <see cref="CreatedFiles"/> property.
        /// </summary>
        [JsonProperty("Errors")]
        private ImmutableArray<ImmutableArray<string>> _errors;
#pragma warning restore 649

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtractArchiveResponse"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected ExtractArchiveResponse()
        {
        }

        /// <summary>
        /// Gets the number of files created by the Extract Archive operation.
        /// </summary>
        /// <value>
        /// The number of files created by the Extract Archive operation.
        /// <para>-or-</para>
        /// <para><see langword="null"/> if the JSON representation did not include the underlying property.</para>
        /// </value>
        public int? CreatedFiles
        {
            get
            {
                return _createdFiles;
            }
        }

        /// <summary>
        /// Gets the response status for the Extract Archive operation.
        /// </summary>
        /// <value>
        /// The response status for the Extract Archive operation.
        /// <para>-or-</para>
        /// <para><see langword="null"/> if the JSON representation did not include the underlying property.</para>
        /// </value>
        public string ResponseStatus
        {
            get
            {
                return _responseStatus;
            }
        }

        /// <summary>
        /// Gets the response body for the Extract Archive operation.
        /// </summary>
        /// <value>
        /// The response body for the Extract Archive operation.
        /// <para>-or-</para>
        /// <para><see langword="null"/> if the JSON representation did not include the underlying property.</para>
        /// </value>
        public string ResponseBody
        {
            get
            {
                return _responseBody;
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="ExtractArchiveError"/> objects describing errors
        /// which occurred while processing specific files within the archive.
        /// </summary>
        /// <value>
        /// A collection of errors, if any, which occurred for specific files during the
        /// Extract Archive operation.
        /// <para>-or-</para>
        /// <para><see langword="null"/> if the JSON representation did not include the underlying property.</para>
        /// </value>
        public ImmutableArray<ExtractArchiveError> Errors
        {
            get
            {
                if (_errors.IsDefault)
                    return default(ImmutableArray<ExtractArchiveError>);

                ImmutableArray<ExtractArchiveError>.Builder errors = ImmutableArray.CreateBuilder<ExtractArchiveError>();
                foreach (ImmutableArray<string> error in _errors)
                {
                    if (error.IsDefault)
                        continue;

                    if (error.Length >= 2)
                        errors.Add(new ExtractArchiveError(error[0], error[1]));
                    else if (error.Length == 1)
                        errors.Add(new ExtractArchiveError(error[0], "Unknown Error"));
                    else
                        errors.Add(new ExtractArchiveError("Unknown File", "Unknown Error"));
                }

                return errors.ToImmutable();
            }
        }
    }
}
