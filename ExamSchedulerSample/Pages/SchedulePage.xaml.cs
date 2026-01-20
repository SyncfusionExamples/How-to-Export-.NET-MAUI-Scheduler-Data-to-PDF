
using System;
using Microsoft.Maui.Controls;

namespace ExamSchedulerSample
{
    public partial class SchedulePage : ContentPage
    {
        public SchedulePage() => InitializeComponent();

        private async void OnExportPdfClicked(object sender, EventArgs e)
            => await Services.PdfExportService.ExportAllAsync();

        private async void OnPreviewPrintClicked(object sender, EventArgs e)
            => await Services.PdfExportService.PreviewAndPrintAsync(Navigation);
    }
}
