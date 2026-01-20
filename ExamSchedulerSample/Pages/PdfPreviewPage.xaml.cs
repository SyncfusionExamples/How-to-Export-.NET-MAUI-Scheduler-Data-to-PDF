using System.IO;
using Microsoft.Maui.Controls;

namespace ExamSchedulerSample
{
    public partial class PdfPreviewPage : ContentPage
    {
        public PdfPreviewPage(Stream pdfStream)
        {
            InitializeComponent();
            Viewer.LoadDocument(pdfStream);
        }

        private void OnPrintClicked(object sender, System.EventArgs e)
        {
            Viewer.PrintDocument(); // opens OS print dialog
        }
    }
}
