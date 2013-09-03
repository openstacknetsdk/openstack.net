namespace net.openstack.Core.Domain
{
    using System;
    using System.Collections.Concurrent;
    using net.openstack.Core.Providers;

    /// <summary>
    /// Represents the state of a compute image.
    /// </summary>
    /// <remarks>
    /// This class functions as a strongly-typed enumeration of known image states,
    /// with added support for unknown states returned by an image extension.
    /// </remarks>
    public sealed class ImageState : IEquatable<ImageState>
    {
        private static readonly ConcurrentDictionary<string, ImageState> _states =
            new ConcurrentDictionary<string, ImageState>(StringComparer.OrdinalIgnoreCase);
        private static readonly ImageState _active = FromName("ACTIVE");
        private static readonly ImageState _build = FromName("BUILD");
        private static readonly ImageState _deleted = FromName("DELETED");
        private static readonly ImageState _error = FromName("ERROR");
        private static readonly ImageState _hardReboot = FromName("HARD_REBOOT");
        private static readonly ImageState _migrating = FromName("MIGRATING");
        private static readonly ImageState _password = FromName("PASSWORD");
        private static readonly ImageState _reboot = FromName("REBOOT");
        private static readonly ImageState _rebuild = FromName("REBUILD");
        private static readonly ImageState _rescue = FromName("RESCUE");
        private static readonly ImageState _resize = FromName("RESIZE");
        private static readonly ImageState _revertResize = FromName("REVERT_RESIZE");
        private static readonly ImageState _suspended = FromName("SUSPENDED");
        private static readonly ImageState _unknown = FromName("UNKNOWN");
        private static readonly ImageState _verifyResize = FromName("VERIFY_RESIZE");
        private static readonly ImageState _prepRescue = FromName("PREP_RESCUE");
        private static readonly ImageState _prepUnrescue = FromName("PREP_UNRESCUE");

        private readonly string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageState"/> class with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        private ImageState(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            _name = name;
        }

        /// <summary>
        /// Gets the <see cref="ImageState"/> instance with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        public static ImageState FromName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            return _states.GetOrAdd(name, i => new ImageState(i));
        }

        /// <summary>
        /// Gets an <see cref="ImageState"/> representing an image which is active and ready to use.
        /// </summary>
        public static ImageState Active
        {
            get
            {
                return _active;
            }
        }

        /// <summary>
        /// Gets an <see cref="ImageState"/> representing an image which is currently being built.
        /// </summary>
        public static ImageState Build
        {
            get
            {
                return _build;
            }
        }

        /// <summary>
        /// Gets an <see cref="ImageState"/> representing an image which has been deleted.
        /// </summary>
        /// <remarks>
        /// By default, the <see cref="IComputeProvider.ListImages"/> operation does not return
        /// images which have been deleted. To list deleted images, call
        /// <see cref="IComputeProvider.ListImages"/> specifying the <c>changesSince</c>
        /// parameter.
        /// </remarks>
        public static ImageState Deleted
        {
            get
            {
                return _deleted;
            }
        }

        /// <summary>
        /// Gets an <see cref="ImageState"/> representing an image which failed to perform
        /// an operation and is now in an error state.
        /// </summary>
        public static ImageState Error
        {
            get
            {
                return _error;
            }
        }

        /// <summary>
        /// Gets an <see cref="ImageState"/> representing an image currently performing a hard
        /// reboot. When the reboot operation completes, the image will be in the <see cref="Active"/>
        /// state.
        /// </summary>
        /// <seealso cref="ImageBase.HardReboot"/>
        /// <seealso cref="IComputeProvider.RebootImage"/>
        public static ImageState HardReboot
        {
            get
            {
                return _hardReboot;
            }
        }

        /// <summary>
        /// Gets an <see cref="ImageState"/> representing an image which is currently being moved
        /// from one physical node to another.
        /// </summary>
        /// <remarks>
        /// <note>Image migration is a Rackspace-specific extension.</note>
        /// </remarks>
        public static ImageState Migrating
        {
            get
            {
                return _migrating;
            }
        }

        /// <summary>
        /// Gets an <see cref="ImageState"/> representing the password for the image is being changed.
        /// </summary>
        /// <seealso cref="IComputeProvider.ChangeAdministratorPassword"/>
        public static ImageState Password
        {
            get
            {
                return _password;
            }
        }

        /// <summary>
        /// Gets an <see cref="ImageState"/> representing an image currently performing a soft
        /// reboot. When the reboot operation completes, the image will be in the <see cref="Active"/>
        /// state.
        /// </summary>
        /// <seealso cref="ImageBase.SoftReboot"/>
        /// <seealso cref="IComputeProvider.RebootImage"/>
        public static ImageState Reboot
        {
            get
            {
                return _reboot;
            }
        }

        /// <summary>
        /// Gets an <see cref="ImageState"/> representing an image currently being rebuilt.
        /// When the rebuild operation completes, the image will be in the <see cref="Active"/>
        /// state if the rebuild was successful; otherwise, it will be in the <see cref="Error"/> state
        /// if the rebuild operation failed.
        /// </summary>
        /// <seealso cref="ImageBase.Rebuild"/>
        /// <seealso cref="IComputeProvider.RebuildImage"/>
        public static ImageState Rebuild
        {
            get
            {
                return _rebuild;
            }
        }

        /// <summary>
        /// Gets an <see cref="ImageState"/> representing an image currently in rescue mode.
        /// </summary>
        /// <seealso cref="ImageBase.Rescue"/>
        /// <seealso cref="IComputeProvider.RescueImage"/>
        public static ImageState Rescue
        {
            get
            {
                return _rescue;
            }
        }

        /// <summary>
        /// Gets an <see cref="ImageState"/> representing an image currently executing a resize action.
        /// When the resize operation completes, the image will be in the <see cref="VerifyResize"/>
        /// state if the resize was successful; otherwise, it will be in the <see cref="Active"/> state
        /// if the resize operation failed.
        /// </summary>
        /// <seealso cref="ImageBase.Resize"/>
        /// <seealso cref="IComputeProvider.ResizeImage"/>
        public static ImageState Resize
        {
            get
            {
                return _resize;
            }
        }

        /// <summary>
        /// Gets an <see cref="ImageState"/> representing an image currently executing a revert resize action.
        /// </summary>
        /// <seealso cref="ImageBase.RevertResize"/>
        /// <seealso cref="IComputeProvider.RevertImageResize"/>
        public static ImageState RevertResize
        {
            get
            {
                return _revertResize;
            }
        }

        /// <summary>
        /// Gets an <see cref="ImageState"/> for an image that is currently inactive, either by request or necessity.
        /// </summary>
        public static ImageState Suspended
        {
            get
            {
                return _suspended;
            }
        }

        /// <summary>
        /// Gets an <see cref="ImageState"/> for an image that is currently in an unknown state.
        /// </summary>
        public static ImageState Unknown
        {
            get
            {
                return _unknown;
            }
        }

        /// <summary>
        /// Gets an <see cref="ImageState"/> representing an image which completed a resize operation
        /// and is now waiting for the operation to be confirmed or reverted.
        /// </summary>
        /// <seealso cref="ImageBase.Resize"/>
        /// <seealso cref="IComputeProvider.ResizeImage"/>
        /// <seealso cref="ImageBase.ConfirmResize"/>
        /// <seealso cref="IComputeProvider.ConfirmImageResize"/>
        public static ImageState VerifyResize
        {
            get
            {
                return _verifyResize;
            }
        }

        /// <summary>
        /// Gets an <see cref="ImageState"/> representing an image currently executing a rescue action.
        /// </summary>
        /// <seealso cref="ImageBase.Rescue"/>
        /// <seealso cref="IComputeProvider.RescueImage"/>
        public static ImageState PrepRescue
        {
            get
            {
                return _prepRescue;
            }
        }

        /// <summary>
        /// Gets an <see cref="ImageState"/> representing an image currently executing an un-rescue action.
        /// </summary>
        /// <seealso cref="ImageBase.UnRescue"/>
        /// <seealso cref="IComputeProvider.UnRescueImage"/>
        public static ImageState PrepUnrescue
        {
            get
            {
                return _prepUnrescue;
            }
        }

        /// <summary>
        /// Gets the canonical name of this image state.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <inheritdoc/>
        public bool Equals(ImageState other)
        {
            return this == other;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Name;
        }
    }
}
