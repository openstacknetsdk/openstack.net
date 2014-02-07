namespace net.openstack.Core
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides extension methods for efficiently creating <see cref="Task"/> continuations,
    /// with automatic handling of faulted and cancelled antecedent tasks.
    /// </summary>
    public static class CoreTaskExtensions
    {
        /// <summary>
        /// Synchronously execute a continuation when a task completes successfully.
        /// </summary>
        /// <remarks>
        /// <para>If the antecedent task is cancelled or faulted, the status of the antecedent is
        /// directly applied to the task returned by this method; it is not wrapped in an additional
        /// <see cref="AggregateException"/>.
        /// </para>
        ///
        /// <note type="caller">
        /// Since the continuation is executed synchronously, this method should only be used for
        /// lightweight continuation functions. For non-trivial continuation functions, use <see cref="Chain{TSource, TResult}(Task{TSource}, Func{Task{TSource}, TResult})"/>
        /// instead.
        /// </note>
        /// </remarks>
        /// <typeparam name="TSource">The type of the result produced by the antecedent <see cref="Task{TResult}"/>.</typeparam>
        /// <typeparam name="TResult">The type of the result produced by the continuation <see cref="Task{TResult}"/>.</typeparam>
        /// <param name="task">The antecedent task.</param>
        /// <param name="continuationFunction">The continuation function to execute when <paramref name="task"/> completes successfully.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully,
        /// the <see cref="Task{TResult}.Result"/> property will contain the result returned from the <paramref name="continuationFunction"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="task"/> is <see langword="null"/>.
        /// <para>-or-</para>
        /// <para>If <paramref name="continuationFunction"/> is <see langword="null"/>.</para>
        /// </exception>
        public static Task<TResult> Select<TSource, TResult>(this Task<TSource> task, Func<Task<TSource>, TResult> continuationFunction)
        {
            return task.Select(continuationFunction, false);
        }

        /// <summary>
        /// Synchronously execute a continuation when a task completes. The <paramref name="supportsErrors"/>
        /// parameter specifies whether the continuation is executed if the antecedent task is faulted.
        /// </summary>
        /// <remarks>
        /// <para>If the antecedent task is cancelled, or faulted with <paramref name="supportsErrors"/>
        /// set to <see langword="false"/>, the status of the antecedent is directly applied to the task
        /// returned by this method; it is not wrapped in an additional <see cref="AggregateException"/>.
        /// </para>
        ///
        /// <note type="caller">
        /// Since the continuation is executed synchronously, this method should only be used for
        /// lightweight continuation functions. For non-trivial continuation functions, use <see cref="Chain{TSource, TResult}(Task{TSource}, Func{Task{TSource}, TResult}, bool)"/>
        /// instead.
        /// </note>
        /// </remarks>
        /// <typeparam name="TSource">The type of the result produced by the antecedent <see cref="Task{TResult}"/>.</typeparam>
        /// <typeparam name="TResult">The type of the result produced by the continuation <see cref="Task{TResult}"/>.</typeparam>
        /// <param name="task">The antecedent task.</param>
        /// <param name="continuationFunction">The continuation function to execute when <paramref name="task"/> completes successfully.</param>
        /// <param name="supportsErrors"><see langword="true"/> if the <paramref name="continuationFunction"/> properly handles a faulted antecedent task; otherwise, <see langword="false"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully,
        /// the <see cref="Task{TResult}.Result"/> property will contain the result returned from the <paramref name="continuationFunction"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="task"/> is <see langword="null"/>.
        /// <para>-or-</para>
        /// <para>If <paramref name="continuationFunction"/> is <see langword="null"/>.</para>
        /// </exception>
        public static Task<TResult> Select<TSource, TResult>(this Task<TSource> task, Func<Task<TSource>, TResult> continuationFunction, bool supportsErrors)
        {
            if (task == null)
                throw new ArgumentNullException("task");
            if (continuationFunction == null)
                throw new ArgumentNullException("continuationFunction");

            TaskCompletionSource<TResult> completionSource = new TaskCompletionSource<TResult>();

            TaskContinuationOptions successContinuationOptions = supportsErrors ? TaskContinuationOptions.NotOnCanceled : TaskContinuationOptions.OnlyOnRanToCompletion;
            task
                .ContinueWith(continuationFunction, TaskContinuationOptions.ExecuteSynchronously | successContinuationOptions)
                .ContinueWith(
                    t =>
                    {
                        if (task.Status == TaskStatus.RanToCompletion || supportsErrors && task.Status == TaskStatus.Faulted)
                            completionSource.SetFromTask(t);
                    }, TaskContinuationOptions.ExecuteSynchronously);

            TaskContinuationOptions failedContinuationOptions = supportsErrors ? TaskContinuationOptions.OnlyOnCanceled : TaskContinuationOptions.NotOnRanToCompletion;
            task
                .ContinueWith(t => completionSource.SetFromTask(t), TaskContinuationOptions.ExecuteSynchronously | failedContinuationOptions);

            return completionSource.Task;
        }

        /// <summary>
        /// Execute a continuation task when a task completes successfully. The continuation
        /// task is synchronously created by a continuation function, and then unwrapped to
        /// form the result of this method.
        /// </summary>
        /// <remarks>
        /// <para>If the antecedent <paramref name="task"/> is cancelled or faulted, the status
        /// of the antecedent is directly applied to the task returned by this method; it is
        /// not wrapped in an additional <see cref="AggregateException"/>.
        /// </para>
        ///
        /// <note type="caller">
        /// Since the <paramref name="continuationFunction"/> is executed synchronously, this
        /// method should only be used for lightweight continuation functions. For non-trivial
        /// continuation functions, use <see cref="ChainAsync{TSource, TResult}(Task{TSource}, Func{Task{TSource}, Task{TResult}})"/> instead. This restriction applies
        /// only to <paramref name="continuationFunction"/> itself, not to the <see cref="Task"/>
        /// returned by it.
        /// </note>
        /// </remarks>
        /// <typeparam name="TSource">The type of the result produced by the antecedent <see cref="Task{TResult}"/>.</typeparam>
        /// <typeparam name="TResult">The type of the result produced by the continuation <see cref="Task{TResult}"/>.</typeparam>
        /// <param name="task">The antecedent task.</param>
        /// <param name="continuationFunction">The continuation function to execute when <paramref name="task"/> completes successfully. The continuation function returns a <see cref="Task{TResult}"/> which provides the final result of the continuation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully,
        /// the <see cref="Task{TResult}.Result"/> property will contain the result provided by the
        /// <see cref="Task{TResult}.Result"/> property of the task returned from <paramref name="continuationFunction"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="task"/> is <see langword="null"/>.
        /// <para>-or-</para>
        /// <para>If <paramref name="continuationFunction"/> is <see langword="null"/>.</para>
        /// </exception>
        public static Task<TResult> SelectAsync<TSource, TResult>(this Task<TSource> task, Func<Task<TSource>, Task<TResult>> continuationFunction)
        {
            if (task == null)
                throw new ArgumentNullException("task");
            if (continuationFunction == null)
                throw new ArgumentNullException("continuationFunction");

            TaskCompletionSource<TResult> completionSource = new TaskCompletionSource<TResult>();

            task
                .ContinueWith(continuationFunction, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion)
                .Unwrap()
                .ContinueWith(
                    t =>
                    {
                        if (task.Status == TaskStatus.RanToCompletion)
                            completionSource.SetFromTask(t);
                    }, TaskContinuationOptions.ExecuteSynchronously);

            task
                .ContinueWith(t => completionSource.SetFromTask(t), TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.NotOnRanToCompletion);

            return completionSource.Task;
        }

        /// <summary>
        /// Execute a continuation when a task completes successfully.
        /// </summary>
        /// <remarks>
        /// <para>If the antecedent task is cancelled or faulted, the status of the antecedent is
        /// directly applied to the task returned by this method; it is not wrapped in an additional
        /// <see cref="AggregateException"/>.
        /// </para>
        /// </remarks>
        /// <typeparam name="TSource">The type of the result produced by the antecedent <see cref="Task{TResult}"/>.</typeparam>
        /// <typeparam name="TResult">The type of the result produced by the continuation <see cref="Task{TResult}"/>.</typeparam>
        /// <param name="task">The antecedent task.</param>
        /// <param name="continuationFunction">The continuation function to execute when <paramref name="task"/> completes successfully.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully,
        /// the <see cref="Task{TResult}.Result"/> property will contain the result returned from the <paramref name="continuationFunction"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="task"/> is <see langword="null"/>.
        /// <para>-or-</para>
        /// <para>If <paramref name="continuationFunction"/> is <see langword="null"/>.</para>
        /// </exception>
        public static Task<TResult> Chain<TSource, TResult>(this Task<TSource> task, Func<Task<TSource>, TResult> continuationFunction)
        {
            return task.Chain(continuationFunction, false);
        }

        /// <summary>
        /// Execute a continuation when a task completes. The <paramref name="supportsErrors"/>
        /// parameter specifies whether the continuation is executed if the antecedent task is faulted.
        /// </summary>
        /// <remarks>
        /// <para>If the antecedent task is cancelled, or faulted with <paramref name="supportsErrors"/>
        /// set to <see langword="false"/>, the status of the antecedent is directly applied to the task
        /// returned by this method; it is not wrapped in an additional <see cref="AggregateException"/>.
        /// </para>
        /// </remarks>
        /// <typeparam name="TSource">The type of the result produced by the antecedent <see cref="Task{TResult}"/>.</typeparam>
        /// <typeparam name="TResult">The type of the result produced by the continuation <see cref="Task{TResult}"/>.</typeparam>
        /// <param name="task">The antecedent task.</param>
        /// <param name="continuationFunction">The continuation function to execute when <paramref name="task"/> completes successfully.</param>
        /// <param name="supportsErrors"><see langword="true"/> if the <paramref name="continuationFunction"/> properly handles a faulted antecedent task; otherwise, <see langword="false"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully,
        /// the <see cref="Task{TResult}.Result"/> property will contain the result returned from the <paramref name="continuationFunction"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="task"/> is <see langword="null"/>.
        /// <para>-or-</para>
        /// <para>If <paramref name="continuationFunction"/> is <see langword="null"/>.</para>
        /// </exception>
        public static Task<TResult> Chain<TSource, TResult>(this Task<TSource> task, Func<Task<TSource>, TResult> continuationFunction, bool supportsErrors)
        {
            if (task == null)
                throw new ArgumentNullException("task");
            if (continuationFunction == null)
                throw new ArgumentNullException("continuationFunction");

            TaskCompletionSource<TResult> completionSource = new TaskCompletionSource<TResult>();

            TaskContinuationOptions successContinuationOptions = supportsErrors ? TaskContinuationOptions.NotOnCanceled : TaskContinuationOptions.OnlyOnRanToCompletion;
            task
                .ContinueWith(continuationFunction, successContinuationOptions)
                .ContinueWith(
                    t =>
                    {
                        if (task.Status == TaskStatus.RanToCompletion || supportsErrors && task.Status == TaskStatus.Faulted)
                            completionSource.SetFromTask(t);
                    }, TaskContinuationOptions.ExecuteSynchronously);

            TaskContinuationOptions failedContinuationOptions = supportsErrors ? TaskContinuationOptions.OnlyOnCanceled : TaskContinuationOptions.NotOnRanToCompletion;
            task
                .ContinueWith(t => completionSource.SetFromTask(t), TaskContinuationOptions.ExecuteSynchronously | failedContinuationOptions);

            return completionSource.Task;
        }

        /// <summary>
        /// Execute a continuation task when a task completes successfully. The continuation
        /// task is created by a continuation function, and then unwrapped to form the result
        /// of this method.
        /// </summary>
        /// <remarks>
        /// <para>If the antecedent <paramref name="task"/> is cancelled or faulted, the status
        /// of the antecedent is directly applied to the task returned by this method; it is
        /// not wrapped in an additional <see cref="AggregateException"/>.
        /// </para>
        /// </remarks>
        /// <typeparam name="TSource">The type of the result produced by the antecedent <see cref="Task{TResult}"/>.</typeparam>
        /// <typeparam name="TResult">The type of the result produced by the continuation <see cref="Task{TResult}"/>.</typeparam>
        /// <param name="task">The antecedent task.</param>
        /// <param name="continuationFunction">The continuation function to execute when <paramref name="task"/> completes successfully. The continuation function returns a <see cref="Task{TResult}"/> which provides the final result of the continuation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully,
        /// the <see cref="Task{TResult}.Result"/> property will contain the result provided by the
        /// <see cref="Task{TResult}.Result"/> property of the task returned from <paramref name="continuationFunction"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="task"/> is <see langword="null"/>.
        /// <para>-or-</para>
        /// <para>If <paramref name="continuationFunction"/> is <see langword="null"/>.</para>
        /// </exception>
        public static Task<TResult> ChainAsync<TSource, TResult>(this Task<TSource> task, Func<Task<TSource>, Task<TResult>> continuationFunction)
        {
            return task.ChainAsync(continuationFunction, false);
        }

        /// <summary>
        /// Execute a continuation task when a task completes. The continuation
        /// task is created by a continuation function, and then unwrapped to form the result
        /// of this method. The <paramref name="supportsErrors"/> parameter specifies whether
        /// the continuation is executed if the antecedent task is faulted.
        /// </summary>
        /// <remarks>
        /// <para>If the antecedent <paramref name="task"/> is cancelled, or faulted with
        /// <paramref name="supportsErrors"/> set to <see langword="false"/>, the status
        /// of the antecedent is directly applied to the task returned by this method; it is
        /// not wrapped in an additional <see cref="AggregateException"/>.
        /// </para>
        /// </remarks>
        /// <typeparam name="TSource">The type of the result produced by the antecedent <see cref="Task{TResult}"/>.</typeparam>
        /// <typeparam name="TResult">The type of the result produced by the continuation <see cref="Task{TResult}"/>.</typeparam>
        /// <param name="task">The antecedent task.</param>
        /// <param name="continuationFunction">The continuation function to execute when <paramref name="task"/> completes successfully. The continuation function returns a <see cref="Task{TResult}"/> which provides the final result of the continuation.</param>
        /// <param name="supportsErrors"><see langword="true"/> if the <paramref name="continuationFunction"/> properly handles a faulted antecedent task; otherwise, <see langword="false"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully,
        /// the <see cref="Task{TResult}.Result"/> property will contain the result provided by the
        /// <see cref="Task{TResult}.Result"/> property of the task returned from <paramref name="continuationFunction"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="task"/> is <see langword="null"/>.
        /// <para>-or-</para>
        /// <para>If <paramref name="continuationFunction"/> is <see langword="null"/>.</para>
        /// </exception>
        public static Task<TResult> ChainAsync<TSource, TResult>(this Task<TSource> task, Func<Task<TSource>, Task<TResult>> continuationFunction, bool supportsErrors)
        {
            if (task == null)
                throw new ArgumentNullException("task");
            if (continuationFunction == null)
                throw new ArgumentNullException("continuationFunction");

            TaskCompletionSource<TResult> completionSource = new TaskCompletionSource<TResult>();

            TaskContinuationOptions successContinuationOptions = supportsErrors ? TaskContinuationOptions.NotOnCanceled : TaskContinuationOptions.OnlyOnRanToCompletion;
            task
                .ContinueWith(continuationFunction, successContinuationOptions)
                .Unwrap()
                .ContinueWith(
                    t =>
                    {
                        if (task.Status == TaskStatus.RanToCompletion || supportsErrors && task.Status == TaskStatus.Faulted)
                            completionSource.SetFromTask(t);
                    }, TaskContinuationOptions.ExecuteSynchronously);

            TaskContinuationOptions failedContinuationOptions = supportsErrors ? TaskContinuationOptions.OnlyOnCanceled : TaskContinuationOptions.NotOnRanToCompletion;
            task
                .ContinueWith(t => completionSource.SetFromTask(t), TaskContinuationOptions.ExecuteSynchronously | failedContinuationOptions);

            return completionSource.Task;
        }
    }
}
