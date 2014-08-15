Imports System.Threading.Tasks
Imports Rackspace.Threading

Public Class AsynchronousExceptionsExamples

    Public Sub ExceptionPriorToTaskCreation()
        ' #Region ExceptionPriorToTaskCreation
        Try
            Dim myTask As Task = SomeOperationAsync()
        Catch ex As ArgumentException
            ' ex was thrown directly by SomeOperationAsync. If SomeOperationAsync Is marked with the `Async`
            ' keyword, then ex was thrown prior to the first use of the `Await` keyword within the implementation.
        End Try
        ' #End Region
    End Sub

    Public Sub ExceptionDuringTaskExecution()
        ' #Region ExceptionDuringTaskExecution
        Try
            Dim myTask As Task = SomeOperationAsync()
        Catch wrapperEx As AggregateException
            Dim ex = TryCast(wrapperEx.InnerException, ArgumentException)
            If ex Is Nothing Then
                Throw
            End If

            ' ex was thrown during the asynchronous portion of SomeOperationAsync. If SomeOperationAsync Is marked
            ' with the `Async` keyword, then ex was thrown after the first use of the `Await` keyword within the
            ' method.
        End Try
        ' #End Region
    End Sub

    Public Sub AsynchronousMethodAsContinuation()
        ' #Region AsynchronousMethodAsContinuation
        ' original asynchronous method invocation
        Dim task1 = SomeOperationAsync()

        ' method invocation treated as a continuation
        Dim task2 = CompletedTask.Default.Then(Function(task) SomeOperationAsync())
        ' #End Region
    End Sub

    Public Function SomeOperationAsync() As Task
        Throw New NotSupportedException()
    End Function

End Class
