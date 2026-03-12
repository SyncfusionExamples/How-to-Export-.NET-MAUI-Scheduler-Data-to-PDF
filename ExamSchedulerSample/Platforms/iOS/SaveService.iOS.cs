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
        private static UIDocumentInteractionController? _docController;

        public static async partial Task SaveAndViewAsync(string filename, string contentType, MemoryStream stream)
        {
            var folder = FileSystem.CacheDirectory;
            var path = Path.Combine(folder, filename);

            Directory.CreateDirectory(folder);

            stream.Position = 0;
            await File.WriteAllBytesAsync(path, stream.ToArray());

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Exported file not found for preview.", path);
            }

            var url = NSUrl.FromFilename(path);
            _docController = UIDocumentInteractionController.FromUrl(url);
            _docController.Delegate = new DocPreviewDelegate();

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                var presenter = GetTopViewController() ?? throw new InvalidOperationException("No presenter UIViewController found.");
                _docController.PresentPreview(true);
            });
        }

        private sealed class DocPreviewDelegate : UIDocumentInteractionControllerDelegate
        {
            public override UIViewController ViewControllerForPreview(UIDocumentInteractionController controller)
                => GetTopViewController() ?? throw new InvalidOperationException("No presenter.");

            public override UIView ViewForPreview(UIDocumentInteractionController controller)
                => GetTopViewController()?.View ?? throw new InvalidOperationException("No presenter view.");

            public override CGRect RectangleForPreview(UIDocumentInteractionController controller)
                => GetTopViewController()?.View.Bounds ?? CGRect.Empty;

            public override void DidEndPreview(UIDocumentInteractionController controller)
            {
                // Release the strong reference
                _docController = null;
            }
        }

        private static UIViewController? GetTopViewController()
        {
            var scene = UIApplication.SharedApplication.ConnectedScenes.OfType<UIWindowScene>().FirstOrDefault();
            var window = scene?.Windows.FirstOrDefault(w => w.IsKeyWindow);
            var vc = window?.RootViewController;
            while (vc?.PresentedViewController != null)
            {
                vc = vc.PresentedViewController;
            }

            return vc;
        }
    }
}