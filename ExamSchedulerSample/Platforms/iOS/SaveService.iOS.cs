using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using UIKit;

namespace ExamSchedulerSample
{
    /// <summary>
    /// iOS-specific implementation of <see cref="SaveService"/> that saves a file
    /// into the app's documents directory and previews it using
    /// <see cref="UIDocumentInteractionController"/>.
    /// </summary>
    /// <remarks>
    /// The preview is presented with <c>PresentPreview(true)</c>, which will use the best available
    /// preview controller on iOS. If the user has no compatible viewer, the system may prompt for
    /// other options depending on installed apps.
    /// </remarks>
    public static partial class SaveService
    {
        /// <summary>
        /// Saves the provided <paramref name="stream"/> as a file with the given
        /// <paramref name="filename"/> and <paramref name="contentType"/>, and then
        /// displays a preview using the iOS document interaction controller.
        /// </summary>
        /// <param name="filename">The output filename (e.g., <c>ExamSchedule.pdf</c>).</param>
        /// <param name="contentType">The MIME type of the file (e.g., <c>application/pdf</c>).</param>
        /// <param name="stream">The in-memory content to be written and previewed.</param>
        /// <returns>A task representing the asynchronous save and preview operation.</returns>
        public static async partial Task SaveAndViewAsync(string filename, string contentType, MemoryStream stream)
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), filename);

            using (var fs = File.Create(path))
            {
                stream.Position = 0;
                await stream.CopyToAsync(fs);
            }

            var url = NSUrl.FromFilename(path);
            var controller = UIDocumentInteractionController.FromUrl(url);

            // Obtain the top/root view controller for presenting the preview.
            var rootVC = UIApplication.SharedApplication
                .ConnectedScenes
                .OfType<UIWindowScene>()
                .First()
                .Windows
                .First()
                .RootViewController;

            // Note: For advanced scenarios, set controller.Delegate to return the appropriate
            // view controller for preview. The default call below works in most basic cases.
            controller.PresentPreview(true);
        }
    }
}
