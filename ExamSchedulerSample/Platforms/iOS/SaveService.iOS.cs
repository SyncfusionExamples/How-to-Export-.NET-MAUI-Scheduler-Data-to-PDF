using System;
using System.IO;
using System.Threading.Tasks;
using Foundation;
using UIKit;

namespace ExamSchedulerSample;

public static partial class SaveService
{
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

        var rootVC = UIApplication.SharedApplication
            .ConnectedScenes
            .OfType<UIWindowScene>()
            .First()
            .Windows
            .First()
            .RootViewController;

        controller.PresentPreview(true);
    }
}
