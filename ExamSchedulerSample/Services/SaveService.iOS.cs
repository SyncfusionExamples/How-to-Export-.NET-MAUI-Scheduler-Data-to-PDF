using Foundation;
using System;
using System.IO;
using System.Threading.Tasks;
using UIKit;

namespace ExamSchedulerSample.Services
{
    public static partial class SaveService
    {
        public static async partial Task SaveAndViewAsync(string filename, string contentType, MemoryStream stream)
        {
            // Save to app’s Documents (visible in Files app under “On My iPhone/…”, if file sharing is enabled)
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var filePath = Path.Combine(documents, filename);

            using (var fileStream = File.Open(filePath, FileMode.Create, FileAccess.Write))
            {
                stream.Position = 0;
                await stream.CopyToAsync(fileStream);
            }

            var viewController = UIApplication.SharedApplication.KeyWindow?.RootViewController;
            while (viewController?.PresentedViewController != null)
                viewController = viewController.PresentedViewController;

            var preview = new UIDocumentInteractionController(NSUrl.FromFilename(filePath))
            {
                UTI = "com.adobe.pdf"
            };

            // Present preview menu; user can open/print via the share sheet
            var view = viewController?.View;
            var rect = view?.Bounds ?? CoreGraphics.CGRect.Empty;
            preview.PresentOptionsMenu(rect, view, true);
        }
    }
}