namespace CSharpCodeSamples
{
    using System;
    using System.Threading.Tasks;
    using Rackspace.Threading;

    public class AsynchronousExceptionsExamples
    {
        public void ExceptionPriorToTaskCreation()
        {
            #region ExceptionPriorToTaskCreation
            try
            {
                Task myTask = SomeOperationAsync();
            }
            catch (ArgumentException ex)
            {
                // ex was thrown directly by SomeOperationAsync. If SomeOperationAsync is marked with the `async`
                // keyword, then ex was thrown prior to the first use of the `await` keyword within the implementation.
            }
            #endregion
        }

        public void ExceptionDuringTaskExecution()
        {
            #region ExceptionDuringTaskExecution
            try
            {
                Task myTask = SomeOperationAsync();
                myTask.Wait();
            }
            catch (AggregateException wrapperEx)
            {
                ArgumentException ex = wrapperEx.InnerException as ArgumentException;
                if (ex == null)
                    throw;

                // ex was thrown during the asynchronous portion of SomeOperationAsync. If SomeOperationAsync is marked
                // with the `async` keyword, then ex was thrown after the first use of the `await` keyword within the
                // method.
            }
            #endregion
        }

        public void AsynchronousMethodAsContinuation()
        {
            #region AsynchronousMethodAsContinuation
            // original asynchronous method invocation
            Task task1 = SomeOperationAsync();

            // method invocation treated as a continuation
            Task task2 = CompletedTask.Default.Then(_ => SomeOperationAsync());
            #endregion
        }

        private static Task SomeOperationAsync()
        {
            throw new NotSupportedException();
        }
    }
}
