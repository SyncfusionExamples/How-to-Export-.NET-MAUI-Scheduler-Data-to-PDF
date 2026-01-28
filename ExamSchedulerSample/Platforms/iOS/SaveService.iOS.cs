using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using UIKit;
using CoreGraphics;

namespace ExamSchedulerSample
{
    /// <summary>
    /// iOS-specific implementation of SaveService that saves a file
    /// to the app's cache directory and previews it via UIDocumentInteractionController.
    /// </summary>
    public static partial class SaveService
    {
        // Keep a strong reference so it isn't GC'd during preview.
        static UIDocumentInteractionController? _docController;

        public static async partial Task SaveAndViewAsync(string filename, string contentType, MemoryStream stream)
        {
            // 1) Persist to a safe, app-accessible location
            var folder = FileSystem.CacheDirectory; // or FileSystem.AppDataDirectory
            var path = Path.Combine(folder, filename);

            // Ensure directory exists
            Directory.CreateDirectory(folder);

            // 2) Write file and close/flush before preview
            stream.Position = 0;
            await File.WriteAllBytesAsync(path, stream.ToArray());

            if (!File.Exists(path))
                throw new FileNotFoundException("Exported file not found for preview.", path);

            // 3) Create the controller and assign a delegate
            var url = NSUrl.FromFilename(path);
            _docController = UIDocumentInteractionController.FromUrl(url);
            _docController.Delegate = new DocPreviewDelegate();

            // 4) Present on the main thread from the top-most controller
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                var presenter = GetTopViewController()
                                ?? throw new InvalidOperationException("No presenter UIViewController found.");
                // Show inline preview UI
                _docController.PresentPreview(true);
                // If you prefer the "Open In…" menu instead:
                // _docController.PresentOptionsMenu(new CGRect(0,0,1,1), presenter.View, true);
            });
        }

        sealed class DocPreviewDelegate : UIDocumentInteractionControllerDelegate
        {
            public override UIViewController ViewControllerForPreview(UIDocumentInteractionController controller)
                => GetTopViewController() ?? throw new InvalidOperationException("No presenter.");

            public override UIView ViewForPreview(UIDocumentInteractionController controller)
                => GetTopViewController()?.View ?? throw new InvalidOperationException("No presenter view.");

            public override CGRect RectangleForPreview(UIDocumentInteractionController controller)
                => GetTopViewController()?.View.Bounds ?? CGRect.Empty;

            // Optional: when preview ends, you can clear the retained controller
            public override void DidEndPreview(UIDocumentInteractionController controller)
            {
                // Release the strong reference
                _docController = null;
            }
        }

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