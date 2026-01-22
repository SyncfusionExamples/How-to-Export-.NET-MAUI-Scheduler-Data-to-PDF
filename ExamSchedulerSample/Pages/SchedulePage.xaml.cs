using System;
using Microsoft.Maui.Controls;
using ExamSchedulerSample.Services;

namespace ExamSchedulerSample
{
    public partial class SchedulePage : ContentPage
    {
        public SchedulePage()
        {
            InitializeComponent();
        }

        private async void OnExportPdfClicked(object sender, EventArgs e)
        {
            try
            {
                await PdfExportService.ExportAllAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void OnPreviewPrintClicked(object sender, EventArgs e)
        {
            try
            {
                await PdfExportService.PreviewAndPrintAsync(Navigation);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}
