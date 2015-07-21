using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenStack.Synchronous.Extensions
{
    /// <summary>
    /// Extensions to <see cref="Task"/>.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Calls an asynchronous method synchronously. Hopefully without causing deadlocks or mucking up any thrown exceptions.
        /// </summary>
        public static T ForceSynchronous<T>(this Task<T> task)
        {
            return task.ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Calls an asynchronous method synchronously. Hopefully without causing deadlocks or mucking up any thrown exceptions.
        /// </summary>
        public static void ForceSynchronous(this Task task)
        {
            task.ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Waits for a task to complete and returns the result as an enumerable.
        /// </summary>
        /// <typeparam name="TCollection">The type of the collection.</typeparam>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="task">The task.</param>
        /// <returns>The task result as an enumerable.</returns>
        public static async Task<IEnumerable<TItem>> AsEnumerable<TCollection, TItem>(this Task<TCollection> task)
            where TCollection : IEnumerable<TItem>
        {
            TCollection result = await task.ConfigureAwait(false);
            return result;
        }
    }
}