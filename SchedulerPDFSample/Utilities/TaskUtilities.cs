namespace SchedulerPDFSample
{
    /// <summary>
    /// TaskUtilities class.
    /// </summary>
    public static class TaskUtilities
    {
        /// <summary>
        /// Safely executes a Task without awaiting the caller, capturing exceptions via the provided error handler.
        /// </summary>
        /// <param name="task">The Task to execute.</param>
        /// <param name="handler">Optional error handler for exception routing.</param>
        public static async void FireAndForgetSafeAsync(this Task task, IErrorHandler? handler = null)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                handler?.HandleError(exception);
            }
        }
    }
}
