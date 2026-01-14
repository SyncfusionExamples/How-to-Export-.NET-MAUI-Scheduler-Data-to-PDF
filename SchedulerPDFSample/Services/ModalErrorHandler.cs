namespace SchedulerPDFSample
{
    /// <summary>
    /// Modal Error Handler.
    /// </summary>
    public class ModalErrorHandler : IErrorHandler
    {
        private readonly SemaphoreSlim _displaySemaphore = new(1, 1);

        /// <summary>
        /// Displays the provided exception message through a modal alert.
        /// </summary>
        /// <param name="exception">The exception to display.</param>
        public void HandleError(Exception exception)
        {
            DisplayAlertAsync(exception).FireAndForgetSafeAsync();
        }

        /// <summary>
        /// Shows an alert dialog with the error message. Serialized by a semaphore to avoid stacked dialogs.
        /// </summary>
        /// <param name="exception">The exception to display.</param>
        private async Task DisplayAlertAsync(Exception exception)
        {
            try
            {
                await _displaySemaphore.WaitAsync();
                if (Shell.Current is Shell shell)
                {
                    await shell.DisplayAlertAsync("Error", exception.Message, "OK");
                }
            }
            finally
            {
                _displaySemaphore.Release();
            }
        }
    }
}
