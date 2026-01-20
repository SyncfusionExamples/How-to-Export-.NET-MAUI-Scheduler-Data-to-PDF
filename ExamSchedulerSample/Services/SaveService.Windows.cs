
using Microsoft.Maui.Controls.PlatformConfiguration;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace ExamSchedulerSample.Services
{
    public static partial class SaveService
    {
        public static async partial Task SaveAndViewAsync(string filename, string contentType, MemoryStream stream)
        {
            var savePicker = new FileSavePicker();

            // Required in WinUI 3 / MAUI on Windows
            var hwnd = WindowNative.GetWindowHandle(App.Current.Windows[0].Handler.PlatformView);
            InitializeWithWindow.Initialize(savePicker, hwnd);

            savePicker.FileTypeChoices.Add("PDF", new[] { ".pdf" });
            savePicker.SuggestedFileName = filename;

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file is null) return;

            using var outStream = await file.OpenStreamForWriteAsync();
            stream.Position = 0;
            await stream.CopyToAsync(outStream);
            await outStream.FlushAsync();

            await Windows.System.Launcher.LaunchFileAsync(file);
        }
    }
}
