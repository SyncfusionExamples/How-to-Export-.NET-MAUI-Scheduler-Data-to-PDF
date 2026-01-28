using Foundation;
using QuickLook;
using UIKit;                    

namespace ExamSchedulerSample
{
    /// <summary>
    /// Mac Catalyst implementation that saves the PDF to the app container and previews it with Quick Look. Printing/“Save as PDF” is available from the system print dialog when the printing entitlement is enabled.
    /// </summary>
    public static partial class SaveService
    {
        public static async partial Task SaveAndViewAsync(string filename, string contentType, MemoryStream stream)
        {
            var folder = FileSystem.CacheDirectory;
            Directory.CreateDirectory(folder);
            var path = Path.Combine(folder, filename);

            stream.Position = 0;
            await File.WriteAllBytesAsync(path, stream.ToArray());

            if (!File.Exists(path))
                throw new FileNotFoundException("File not found after write.", path);

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                var preview = new QLPreviewController
                {
                    DataSource = new SingleFilePreviewDataSource(path)
                };

                var presenter = GetTopViewController()
                    ?? throw new InvalidOperationException("No presenter UIViewController found.");

                presenter.PresentViewController(preview, true, null);
            });
        }

        /// <summary>
        /// Minimal Quick Look item backed by a file on disk.
        /// IMPORTANT: Inherit from NSObject so INativeObject.Handle/IDisposable are satisfied.
        /// </summary>
        sealed class SingleFilePreviewItem : NSObject, IQLPreviewItem
        {
            public NSUrl ItemUrl { get; }
            public string ItemTitle { get; }

            public SingleFilePreviewItem(string path, string title)
            {
                ItemUrl = NSUrl.FromFilename(path);
                ItemTitle = title;
            }
        }

        /// <summary>
        /// Quick Look data source that exposes a single item.
        /// </summary>
        sealed class SingleFilePreviewDataSource : QLPreviewControllerDataSource
        {
            readonly string _path;

            public SingleFilePreviewDataSource(string path) => _path = path;

            public override nint PreviewItemCount(QLPreviewController controller) => 1;

            public override IQLPreviewItem GetPreviewItem(QLPreviewController controller, nint index)
                => new SingleFilePreviewItem(_path, Path.GetFileName(_path));
        }

        /// <summary>
        /// Finds the top-most UIViewController for presenting UI.
        /// </summary>
        static UIViewController? GetTopViewController()
        {
            var scene = UIApplication.SharedApplication.ConnectedScenes.OfType<UIWindowScene>().FirstOrDefault();
            var window = scene?.Windows.FirstOrDefault(w => w.IsKeyWindow);
            var vc = window?.RootViewController;

            while (vc?.PresentedViewController != null)
                vc = vc.PresentedViewController;

            return vc;
        }
    }
}