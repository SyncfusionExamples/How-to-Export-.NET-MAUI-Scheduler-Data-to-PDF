using Microsoft.Maui.Controls.PlatformConfiguration;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace ExamSchedulerSample
{
    /// <summary>
    /// Windows-specific implementation of <see cref="SaveService"/> that uses a
    /// <see cref="FileSavePicker"/> to save a file and then launches it using the
    /// default associated application on the device.
    /// </summary>
    /// <remarks>
    /// In WinUI 3 / .NET MAUI for Windows, a window handle (HWND) must be associated
    /// with the <see cref="FileSavePicker"/> using
    /// <see cref="InitializeWithWindow.Initialize(object, IntPtr)"/> to display it.
    /// </remarks>
    public static partial class SaveService
    {
        /// <summary>
        /// Prompts the user to save the provided <paramref name="stream"/> as a file with the given
        /// <paramref name="filename"/> and <paramref name="contentType"/> using a Windows
        /// <see cref="FileSavePicker"/>, writes the content, and then opens the saved file.
        /// </summary>
        /// <param name="filename">The suggested file name (e.g., <c>ExamSchedule.pdf</c>).</param>
        /// <param name="contentType">The MIME type of the file (e.g., <c>application/pdf</c>).</param>
        /// <param name="stream">The in-memory file content to persist.</param>
        /// <returns>A task representing the asynchronous save and launch operation.</returns>
        public static async partial Task SaveAndViewAsync(string filename, string contentType, MemoryStream stream)
        {
            var savePicker = new FileSavePicker();

            // Required in WinUI 3 / MAUI on Windows: associate the picker with the current window (HWND).
            var hwnd = WindowNative.GetWindowHandle(App.Current.Windows[0].Handler.PlatformView);
            InitializeWithWindow.Initialize(savePicker, hwnd);

            // Configure file types and suggested file name.
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
