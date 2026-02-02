using Foundation;
using QuickLook;
using UIKit;

namespace ExamSchedulerSample
{
    /// <summary>
    /// Provides Mac Catalyst–specific save, preview, and export functionality.
    /// Implements Quick Look preview and the native "Save As…" export flow.
    /// </summary>
    public static partial class SaveService
    {
        /// <summary>
        /// Saves the provided stream to the app cache directory and presents it using Quick Look.
        /// </summary>
        /// <param name="filename">The name of the file to save (e.g., "ExamSchedule-UG.pdf").</param>
        /// <param name="contentType">The MIME type of the file (e.g., "application/pdf").</param>
        /// <param name="stream">The memory stream containing the file data. It will be read from position 0.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the file does not exist after writing to disk.</exception>
        public static async partial Task SaveAndViewAsync(string filename, string contentType, MemoryStream stream)
        {
            var cacheDirectory = FileSystem.CacheDirectory;
            Directory.CreateDirectory(cacheDirectory);

            var filePath = Path.Combine(cacheDirectory, filename);

            stream.Position = 0;
            await File.WriteAllBytesAsync(filePath, stream.ToArray()).ConfigureAwait(false);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found after write.", filePath);
            }

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                var previewController = new QLPreviewController
                {
                    DataSource = new SingleFilePreviewDataSource(filePath)
                };

                var presentingViewController = GetTopViewController() ?? throw new InvalidOperationException("No presenter UIViewController found.");

                presentingViewController.PresentViewController(previewController, true, null);
            });
        }

        /// <summary>
        /// Presents the native Mac Catalyst "Save As…" dialog to export a file to a user-selected location.
        /// </summary>
        /// <param name="filename">The name of the file to export (e.g., "ExamSchedule-UG.pdf").</param>
        /// <param name="fileBytes">The binary content to export.</param>
        /// <returns>
        /// A task that resolves to <c>true</c> if the export completed successfully,
        /// or <c>false</c> if the user cancelled.
        /// </returns>
        /// <remarks>
        /// Requires entitlement <c>com.apple.security.files.user-selected.read-write</c> for Mac Catalyst.
        /// </remarks>
        public static async Task<bool> ExportAsync(string filename, byte[] fileBytes)
        {
            var sourcePath = Path.Combine(FileSystem.CacheDirectory, filename);
            await File.WriteAllBytesAsync(sourcePath, fileBytes).ConfigureAwait(false);

            var completionSource = new TaskCompletionSource<bool>();

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                UIDocumentPickerViewController? documentPicker = null;

                documentPicker = new UIDocumentPickerViewController(
                    new[] { NSUrl.FromFilename(sourcePath) },
                    UIDocumentPickerMode.ExportToService)
                {
                    ShouldShowFileExtensions = true
                };

                documentPicker.WasCancelled += (_, __) =>
                {
                    try
                    {
                        documentPicker?.DismissViewController(true, null);
                    }
                    finally
                    {
                        completionSource.TrySetResult(false);
                        documentPicker?.Dispose();
                        documentPicker = null;
                    }
                };

                documentPicker.DidPickDocumentAtUrls += (_, __) =>
                {
                    try
                    {
                        documentPicker?.DismissViewController(true, null);
                    }
                    catch (Exception ex)
                    {
                        completionSource.TrySetException(ex);
                        documentPicker?.Dispose();
                        documentPicker = null;
                        return;
                    }

                    completionSource.TrySetResult(true);
                    documentPicker?.Dispose();
                    documentPicker = null;
                };

                var presentingViewController = GetTopViewController() ?? throw new InvalidOperationException("No presenter UIViewController found.");

                presentingViewController.PresentViewController(documentPicker, true, null);
            });

            return await completionSource.Task.ConfigureAwait(false);
        }

        /// <summary>
        /// Represents a single file for Quick Look preview.
        /// </summary>
        sealed class SingleFilePreviewItem : NSObject, IQLPreviewItem
        {
            /// <inheritdoc/>
            public NSUrl ItemUrl { get; }

            /// <inheritdoc/>
            public string ItemTitle { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="SingleFilePreviewItem"/> class.
            /// </summary>
            /// <param name="path">The file system path to preview.</param>
            /// <param name="title">The display title for the preview item.</param>
            public SingleFilePreviewItem(string path, string title)
            {
                ItemUrl = NSUrl.FromFilename(path);
                ItemTitle = title;
            }
        }

        /// <summary>
        /// Quick Look data source for previewing a single file.
        /// </summary>
        sealed class SingleFilePreviewDataSource : QLPreviewControllerDataSource
        {
            readonly string _filePath;

            /// <summary>
            /// Initializes a new instance of the <see cref="SingleFilePreviewDataSource"/> class.
            /// </summary>
            /// <param name="path">The file path to present in Quick Look.</param>
            public SingleFilePreviewDataSource(string path) => _filePath = path;

            /// <inheritdoc/>
            public override nint PreviewItemCount(QLPreviewController controller) => 1;

            /// <inheritdoc/>
            public override IQLPreviewItem GetPreviewItem(QLPreviewController controller, nint index) => new SingleFilePreviewItem(_filePath, Path.GetFileName(_filePath));
        }

        /// <summary>
        /// Gets the top-most view controller suitable for presenting UIKit modals.
        /// </summary>
        /// <returns>The currently visible <see cref="UIViewController"/>, or <c>null</c> if not available.</returns>
        static UIViewController? GetTopViewController()
        {
            var windowScene = UIApplication.SharedApplication.ConnectedScenes
                .OfType<UIWindowScene>()
                .FirstOrDefault();

            var keyWindow = windowScene?.Windows.FirstOrDefault(w => w.IsKeyWindow);
            var viewController = keyWindow?.RootViewController;

            while (viewController?.PresentedViewController != null)
            {
                viewController = viewController.PresentedViewController;
            }

            if (viewController is UINavigationController navigationController)
            {
                viewController = navigationController.VisibleViewController ?? navigationController.TopViewController;
            }

            if (viewController is UITabBarController tabBarController && tabBarController.SelectedViewController != null)
            {
                viewController = tabBarController.SelectedViewController;
            }

            return viewController;
        }
    }
}