namespace ExamSchedulerSample
{
    /// <summary>
    /// Displays the program-specific exam schedule (UG/PG/MBA), and provides actions to export the schedule as PDF or preview/print it.
    /// </summary>
    public partial class SchedulePage : ContentPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulePage"/> class with the default program (<see cref="ProgramType.UG"/>).
        /// </summary>
        public SchedulePage() : this(ProgramType.UG)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulePage"/> class using a string-based program identifier (e.g., "UG", "PG", "MBA").
        /// Falls back to <see cref="ProgramType.UG"/> if parsing fails.
        /// </summary>
        /// <param name="program">The program name (case-insensitive): "UG", "PG", or "MBA".</param>
        public SchedulePage(string program) : this(ParseProgram(program))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulePage"/> class
        /// and binds a <see cref="ScheduleViewModel"/> for the specified program.
        /// </summary>
        /// <param name="program">The selected program (UG, PG, MBA).</param>
        public SchedulePage(ProgramType program)
        {
            InitializeComponent();

            var viewModel = new ScheduleViewModel(program);
            BindingContext = viewModel;

            Title = $"Exam Schedule - {viewModel.Program}";
        }

        /// <summary>
        /// Handles the ExportPDF button click and export's the current program’s schedule.
        /// </summary>
        private async void OnExportPdfClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                button.IsEnabled = false;
            }

            try
            {
                // Safely read the VM and program
                if (BindingContext is not ScheduleViewModel viewModel)
                {
                    await DisplayAlert("Export PDF", "No data context available.", "OK");
                    return;
                }

                var program = viewModel.Program;
                var pdfBytes = await Task.Run(() => PdfExportService.GetPdfBytes(program));
                var fileName = $"ExamSchedule-{program}.pdf";

#if MACCATALYST
                var exported = await SaveService.ExportAsync(fileName, pdfBytes);
                if (exported)
                {
                    await DisplayAlert("Export PDF", "PDF export complete.", "OK");
                }
                else
                {
                    await DisplayAlert("Export PDF", "Export cancelled.", "OK");
                }
#else
                using var ms = new MemoryStream(pdfBytes, writable: false);
                await SaveService.SaveAndViewAsync(fileName, "application/pdf", ms);
#endif
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                if (button != null)
                {
                    button.IsEnabled = true;
                }
            }
        }

        /// <summary>
        /// Handles the Preview/Print button click and opens a preview for the current program’s schedule.
        /// </summary>
        private async void OnPreviewPrintClicked(object sender, EventArgs e)
        {
            try
            {
                var viewModel = BindingContext as ScheduleViewModel;
                var program = viewModel?.Program ?? ProgramType.UG;

                await PdfExportService.PreviewAndPrintAsync(Navigation, program);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        /// <summary>
        /// Parses a string (e.g., "UG", "PG", "MBA") into a <see cref="ProgramType"/>. Defaults to <see cref="ProgramType.UG"/> if parsing fails.
        /// </summary>
        private static ProgramType ParseProgram(string? value)
            => Enum.TryParse<ProgramType>(value, true, out var parsed) ? parsed : ProgramType.UG;
    }
}