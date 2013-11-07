namespace net.openstack.Core
{
    /// <summary>
    /// Defines a provider for progress updates.
    /// </summary>
    /// <typeparam name="T">The type of progress update value.</typeparam>
    /// <preliminary/>
    public interface IProgress<
#if NET35
        T
#else
        in T
#endif
        >
    {
        /// <summary>
        /// Reports a progress update.
        /// </summary>
        /// <param name="value">The value of the updated progress.</param>
        void Report(T value);
    }
}
