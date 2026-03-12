namespace ExamSchedulerSample
{
    /// <summary>
    /// A page that displays a PDF document and provides options to print or close the preview. 
    /// The provided input stream is copied into an owned <see cref="MemoryStream"/> to ensure the content remains available for the lifetime of the page.
    /// </summary>
    public partial class PdfPreviewPage : ContentPage
    {
        /// <summary>
        /// Holds an owned copy of the PDF content so that the viewer can access it safely even if the original stream is disposed. Disposed when the page disappears.
        /// </summary>
        private MemoryStream? _ownedStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPreviewPage"/> class. Accepts any stream (seekable or not), copies its content into an internally owned <see cref="MemoryStream"/>, and loads it into the viewer.
        /// </summary>
        /// <param name="pdfStream">
        /// The source stream containing PDF bytes. It can be seekable or non-seekable.
        /// Its content is copied into an internal <see cref="MemoryStream"/> that this page owns.
        /// </param>
        public PdfPreviewPage(Stream pdfStream)
        {
            InitializeComponent();

            _ownedStream = new MemoryStream();
            pdfStream.Position = pdfStream.CanSeek ? 0 : pdfStream.Position;
            pdfStream.CopyTo(_ownedStream);
            _ownedStream.Position = 0;
            Viewer.LoadDocument(_ownedStream);
        }

        /// <summary>
        /// Unloads the document from the viewer and disposes the owned stream when the page is about to disappear, freeing resources promptly.
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Viewer?.UnloadDocument();
            _ownedStream?.Dispose();
            _ownedStream = null;
        }
    }
}