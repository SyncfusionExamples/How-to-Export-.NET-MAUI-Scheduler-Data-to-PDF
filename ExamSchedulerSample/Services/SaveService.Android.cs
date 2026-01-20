
using System.IO;
using System.Threading.Tasks;
using Android.Content;
using AndroidX.Core.Content;
using Microsoft.Maui.ApplicationModel;

namespace ExamSchedulerSample.Services
{
    public static partial class SaveService
    {
        public static async partial Task SaveAndViewAsync(string filename, string contentType, MemoryStream stream)
        {
            var path = Path.Combine(FileSystem.CacheDirectory, filename);

            using (var fileStream = File.Open(path, FileMode.Create, FileAccess.Write))
            {
                stream.Position = 0;
                await stream.CopyToAsync(fileStream);
            }

            var context = Platform.CurrentActivity ?? Android.App.Application.Context;
            var file = new Java.IO.File(path);
            file.SetReadable(true);

            // Use FileProvider (see manifest setup below)
            var uri = FileProvider.GetUriForFile(context, $"{context.PackageName}.provider", file);

            var intent = new Intent(Intent.ActionView);
            intent.SetDataAndType(uri, contentType);
            intent.SetFlags(ActivityFlags.GrantReadUriPermission | ActivityFlags.NewTask);

            context.StartActivity(intent);
        }
    }
}
