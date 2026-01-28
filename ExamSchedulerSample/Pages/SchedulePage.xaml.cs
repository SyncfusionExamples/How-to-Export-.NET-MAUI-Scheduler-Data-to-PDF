using System;
using Microsoft.Maui.Controls;
using ExamSchedulerSample.Services;

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

            var vm = new ScheduleViewModel(program);
            BindingContext = vm;

            Title = $"Exam Schedule - {vm.Program}";
        }

        /// <summary>
        /// Handles the Export PDF button click and generates a program-specific PDF for the currently bound schedule.
        /// </summary>
        private async void OnExportPdfClicked(object sender, EventArgs e)
        {
            try
            {
                var vm = BindingContext as ScheduleViewModel;
                var program = vm?.Program ?? ProgramType.UG;

                // Prefer program-aware export if available.
                await PdfExportService.ExportAllAsync(program);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        /// <summary>
        /// Handles the Preview/Print button click and opens a preview for the current program’s schedule.
        /// </summary>
        private async void OnPreviewPrintClicked(object sender, EventArgs e)
        {
            try
            {
                var vm = BindingContext as ScheduleViewModel;
                var program = vm?.Program ?? ProgramType.UG;

                // Prefer program-aware preview if available.
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
