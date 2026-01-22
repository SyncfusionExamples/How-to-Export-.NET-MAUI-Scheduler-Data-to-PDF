
using System;
using System.IO;
using Microsoft.Maui.Controls;

namespace ExamSchedulerSample
{
    public partial class PdfPreviewPage : ContentPage
    {
        // Keep a reference so GC does not collect it and so it isn't disposed elsewhere.
        private MemoryStream? _ownedStream;

        /// <summary>
        /// Pass any stream (seekable or not). We will copy it into a MemoryStream
        /// we own and keep it for the lifetime of this page.
        /// </summary>
        public PdfPreviewPage(Stream pdfStream)
        {
            InitializeComponent();

            // Defensive: copy to our own seekable MemoryStream and rewind.
            _ownedStream = new MemoryStream();
            pdfStream.Position = pdfStream.CanSeek ? 0 : pdfStream.Position;
            pdfStream.CopyTo(_ownedStream);
            _ownedStream.Position = 0;

            // Load into the viewer. Do NOT wrap in using; keep _ownedStream alive.
            Viewer.LoadDocument(_ownedStream);
        }

        private void OnPrintClicked(object sender, EventArgs e)
        {
            try
            {
                Viewer?.PrintDocument();
            }
            catch (Exception ex)
            {
                DisplayAlert("Print failed", ex.Message, "OK");
            }
        }

        private async void OnCloseClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            try
            {
                // Unload the document before disposing the stream (if API is present).
                // Some versions expose UnloadDocument; if not available, you can skip it.
                Viewer?.UnloadDocument();
            }
            catch { /* ignore if not supported */ }

            // Now it's safe to dispose our stream.
            _ownedStream?.Dispose();
            _ownedStream = null;
        }
    }
}
