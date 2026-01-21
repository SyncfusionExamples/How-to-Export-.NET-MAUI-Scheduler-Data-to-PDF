using System.IO;
using System.Threading.Tasks;
using Android.Content;
using AndroidX.Core.Content;
using Microsoft.Maui.ApplicationModel;

namespace ExamSchedulerSample;

public static partial class SaveService
{
    public static async partial Task SaveAndViewAsync(string filename, string contentType, MemoryStream stream)
    {
        // Save to app cache
        var path = Path.Combine(FileSystem.CacheDirectory, filename);
        using (var fs = File.Open(path, FileMode.Create, FileAccess.Write))
        {
            stream.Position = 0;
            await stream.CopyToAsync(fs);
        }

        var context = Platform.CurrentActivity ?? Android.App.Application.Context;

        // Use fully-qualified AndroidX FileProvider to avoid ambiguity
        var file = new Java.IO.File(path);
        file.SetReadable(true);
        var uri = AndroidX.Core.Content.FileProvider.GetUriForFile(
            context,
            $"{context.PackageName}.provider",
            file);

        // Build intent
        var intent = new Intent(Intent.ActionView);
        intent.SetDataAndType(uri, contentType);
        intent.SetFlags(ActivityFlags.GrantReadUriPermission | ActivityFlags.NewTask);

        // If no app can handle it, avoid crash and show a chooser as fallback
        var res = intent.ResolveActivity(context.PackageManager);
        if (res is null)
        {
            var chooser = Intent.CreateChooser(intent, "Open with");
            chooser.SetFlags(ActivityFlags.GrantReadUriPermission | ActivityFlags.NewTask);
            context.StartActivity(chooser);
            return;
        }

        try
        {
            context.StartActivity(intent);
        }
        catch (ActivityNotFoundException)
        {
            var chooser = Intent.CreateChooser(intent, "Open with");
            chooser.SetFlags(ActivityFlags.GrantReadUriPermission | ActivityFlags.NewTask);
            context.StartActivity(chooser);
        }
    }
}
