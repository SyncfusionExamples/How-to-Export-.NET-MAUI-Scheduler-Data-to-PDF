namespace ExamSchedulerSample
{
    /// <summary>
    /// Provides an abstraction for saving and viewing files on the current device.
    /// Implementations of this partial class must provide the platform?specific behavior for persisting a file and optionally opening it in a viewer.
    /// </summary>
    public static partial class SaveService
    {
        /// <summary>
        /// Saves the provided stream as a file with the given filename and content type,then opens or previews it using the appropriate application on the device.
        /// This method is implemented separately per platform (Android, iOS, Windows, etc.) via <c>partial</c> class extensions.
        /// </summary>
        /// <param name="filename">The name of the file to save (e.g., "Report.pdf").</param>
        /// <param name="contentType">The MIME type of the file (e.g., "application/pdf").</param>
        /// <param name="stream">The memory stream containing the file’s binary content.</param>
        /// <returns>A task representing the asynchronous save/open operation.</returns>
        public static partial Task SaveAndViewAsync(string filename, string contentType, MemoryStream stream);
    }
}
