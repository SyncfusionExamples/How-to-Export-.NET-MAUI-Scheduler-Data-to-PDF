using System;
using Microsoft.Maui.Controls;
using ExamSchedulerSample.Services;

namespace ExamSchedulerSample;

public partial class SchedulePage : ContentPage
{
    public SchedulePage()
    {
        InitializeComponent();
        ValidateSchedulerData();  // optional: logs issues to Output window
    }

    private async void OnExportPdfClicked(object sender, EventArgs e)
        => await PdfExportService.ExportAllAsync();

    private async void OnPreviewPrintClicked(object sender, EventArgs e)
        => await PdfExportService.PreviewAndPrintAsync(Navigation);

    // ---- Optional validator to spot data issues early ----
    private void ValidateSchedulerData()
    {
        try
        {
            var viewModel = BindingContext as ScheduleViewModel;
            if (viewModel == null) { System.Diagnostics.Debug.WriteLine("[VALIDATE] VM is null"); return; }

            var halls = viewModel.Halls ?? new();
            var hallIds = new HashSet<object>(halls.Select(h => (object)h.Id));
            int i = 0;

            foreach (var a in viewModel.Appointments ?? Enumerable.Empty<ExamAppointment>())
            {
                if (a.Start >= a.End)
                    System.Diagnostics.Debug.WriteLine($"[VALIDATE] Appt[{i}] Start >= End ({a.Start} .. {a.End})");

                // If you later enable ResourceView, uncomment this section.
                // if (a.ResourceIds == null)
                //     System.Diagnostics.Debug.WriteLine($"[VALIDATE] Appt[{i}] ResourceIds is null");
                //
                // if (a.ResourceIds != null)
                //     foreach (var rid in a.ResourceIds)
                //         if (!hallIds.Contains(rid))
                //             System.Diagnostics.Debug.WriteLine($"[VALIDATE] Appt[{i}] ResourceId '{rid}' not found in Halls");

                i++;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("[VALIDATE] threw: " + ex);
        }
    }
}
