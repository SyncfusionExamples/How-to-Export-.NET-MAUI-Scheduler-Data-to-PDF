using Android.Content;
using AndroidX.Core.Content;

namespace ExamSchedulerSample
{
    /// <summary>
    /// Android-specific implementation of <see cref="SaveService"/> that saves a file
    /// to the app's cache directory and launches an appropriate viewer using an
    /// <see cref="Intent"/> with a content <c>Uri</c> from <see cref="FileProvider"/>.
    /// </summary>
    public static partial class SaveService
    {
        /// <summary>
        /// Saves the provided <paramref name="stream"/> to the app's cache directory with the given
        /// <paramref name="filename"/> and <paramref name="contentType"/>, then launches an
        /// appropriate viewer via an <see cref="Intent.ActionView"/> intent using a
        /// <see cref="FileProvider"/> content <c>Uri</c>.
        /// </summary>
        /// <param name="filename">The output file name (e.g., <c>ExamSchedule.pdf</c>).</param>
        /// <param name="contentType">The MIME type of the file (e.g., <c>application/pdf</c>).</param>
        /// <param name="stream">The in-memory content to persist and view.</param>
        /// <returns>A task representing the asynchronous save and view operation.</returns>
        public static async partial Task SaveAndViewAsync(string filename, string contentType, MemoryStream stream)
        {
            var path = Path.Combine(FileSystem.CacheDirectory, filename);
            using (var fs = File.Open(path, FileMode.Create, FileAccess.Write))
            {
                stream.Position = 0;
                await stream.CopyToAsync(fs);
            }

            var context = Platform.CurrentActivity ?? Android.App.Application.Context;

            var file = new Java.IO.File(path);
            file.SetReadable(true);
            var uri = AndroidX.Core.Content.FileProvider.GetUriForFile(
                context,
                $"{context.PackageName}.provider",
                file);

            var intent = new Intent(Intent.ActionView);
            intent.SetDataAndType(uri, contentType);
            intent.SetFlags(ActivityFlags.GrantReadUriPermission | ActivityFlags.NewTask);

            var handler = intent.ResolveActivity(context.PackageManager);
            if (handler is null)
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
}
