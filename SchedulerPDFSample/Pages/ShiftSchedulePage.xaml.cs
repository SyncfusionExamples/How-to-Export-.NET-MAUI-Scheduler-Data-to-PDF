using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using PointF = Syncfusion.Drawing.PointF;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Extensions.DependencyInjection;
using Color = Microsoft.Maui.Graphics.Color;

namespace SchedulerPDFSample;

/// <summary>
/// Page that displays a weekly shift schedule using Syncfusion Scheduler and
/// provides a toolbar action to export the visible week's handout to PDF.
/// </summary>
/// <summary>
/// A page that shows a weekly scheduler view and allows exporting the visible week to a PDF handout.
/// </summary>
public partial class ShiftSchedulePage : ContentPage
{
    #region Fields
    /// <summary>
    /// In-memory appointment collection used as the Scheduler's ItemsSource.
    /// </summary>
    private readonly ObservableCollection<Models.CustomSchedulerAppointment> _appointments = new();
    #endregion

    #region Constructor
    /// <summary>
    /// Initializes the page and binds the Scheduler.
    /// </summary>
    

    /// <summary>
    /// Initializes the scheduler view and binds the appointment collection.
    /// </summary>
    public ShiftSchedulePage()
    {
        InitializeComponent();
        Scheduler.View = SchedulerView.Week;
        Scheduler.DisplayDate = DateTime.Today;
        Scheduler.AppointmentsSource = _appointments;
    }

    /// <summary>
    /// Triggers initial appointment load when the page appears.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadAppointmentsAsync().FireAndForgetSafeAsync();
    }
    #endregion

    #region Data
    /// <summary>
    /// Loads tasks from the repository into the Scheduler, excluding completed tasks.
    /// </summary>
    /// <summary>
    /// Loads tasks from repositories and projects them into Scheduler appointments with category colors.
    /// </summary>
    /// <returns>A task representing the asynchronous load operation.</returns>
    private async Task LoadAppointmentsAsync()
    {
        _appointments.Clear();

        // Resolve repositories via DI from the current MauiContext
        var services = Application.Current?.Handler?.MauiContext?.Services;
        if (services is null)
        {
            return; // page not wired yet
        }
        var taskRepo = services.GetRequiredService<TaskRepository>();
        var categoryRepo = services.GetRequiredService<CategoryRepository>();

        var tasks = await taskRepo.ListAsync();
        var categories = await categoryRepo.ListAsync();

        // Build category palette from Manage Meta
        var categoryColors = categories.ToDictionary(category => category.Title, category => category.Color);
        if (categoryColors.Count == 0)
        {
            categoryColors = new Dictionary<string, string>
            {
                { "work", "#3068df" },
                { "education", "#8800FF" },
                { "self", "#FF3300" },
                { "relationships", "#FF9900" },
                { "personal", "#32CD32" },
                { "family", "#1E90FF" }
            };
        }
        var categoryList = categoryColors.Keys.ToList();

        // Compute default schedule grid across the current week to avoid same time slots
        var weekStart = FirstDayOfWeek().AddHours(9); // start at 9 AM

        // Note: Demo data removed since app uses seeded/DB tasks. If DB is empty, no fallback is added.
        if (tasks.Count == 0)
        {
            return;
        }

        int slotIndex = 0;

        foreach (var task in tasks)
        {
            // Category assignment (use existing, else cycle)
            // Category assignment (use existing, else cycle from palette)
            var assignedCategory = !string.IsNullOrWhiteSpace(task.Category)
                ? task.Category
                : categoryList[slotIndex % categoryList.Count];

            // Time assignment if missing: spread across week at 9, 11, 13, 15
            var start = task.StartTime == default
                ? weekStart.Date.AddDays(slotIndex % 7).AddHours(9 + (slotIndex % 4) * 2)
                : task.StartTime;
            var end = task.EndTime == default || task.EndTime <= task.StartTime
                ? start.AddHours(1)
                : task.EndTime;

            var baseHex = categoryColors.TryGetValue(assignedCategory, out var hex) ? hex : "#5D9CEC";
            var brushColor = Color.FromArgb(baseHex);
            if (task.IsCompleted)
            {
                // Dim completed tasks
                brushColor = brushColor.WithAlpha(0.4f);
            }

            _appointments.Add(new Models.CustomSchedulerAppointment
            {
                Subject    = string.IsNullOrWhiteSpace(task.Title) ? "Task" : (task.IsCompleted ? "\u2713 " + task.Title : task.Title),
                StartTime  = start,
                EndTime    = end,
                Background = new SolidColorBrush(brushColor),
                Notes      = task.IsCompleted ? $"{assignedCategory} (Completed)" : assignedCategory,
                Category   = assignedCategory
            });

            slotIndex++;
        }
    }
    #endregion

    #region Export
    // Removed sample SeedAppointments; using LoadAppointmentsAsync to populate from TaskRepository.
    #endregion

    #region Event Handlers
    /// <summary>
    /// Handles toolbar taps for Back, Today, and Export PDF actions.
    /// </summary>
    /// <param name="sender">Toolbar instance.</param>
    /// <param name="e">Tap event args containing the tapped item information.</param>
    /// <summary>
    /// Handles scheduler toolbar taps and invokes PDF export when requested.
    /// </summary>
    /// <param name="sender">The toolbar originating the event.</param>
    /// <param name="e">Tap event arguments.</param>
    private async void OnToolbarTapped(object? sender, ToolbarTappedEventArgs e)
    {
        if (e.NewToolbarItem?.Name == "ExportPdfButton")
        {
            await ExportPdfAsync();
        }
    }
    #endregion

    #region Export
    /// <summary>
    /// Generates a PDF handout for the current week, listing shift appointments
    /// with times, subjects, locations, and notes, then shares the file.
    /// </summary>
    /// <summary>
    /// Creates a printable PDF with the current week's appointments grouped by day and shares the file.
    /// </summary>
    /// <returns>A task that completes after the PDF is created and shared.</returns>
    private async Task ExportPdfAsync()
    {
        using var pdfDocument = new PdfDocument();
        var pdfPage = pdfDocument.Pages.Add();
        var graphics = pdfPage.Graphics;

        var titleFont = new PdfStandardFont(PdfFontFamily.Helvetica, 18, PdfFontStyle.Bold);
        var headerFont = new PdfStandardFont(PdfFontFamily.Helvetica, 12, PdfFontStyle.Bold);
        var textFont = new PdfStandardFont(PdfFontFamily.Helvetica, 11);

        // Title
        graphics.DrawString("Weekly Schedule", titleFont, PdfBrushes.Black, new PointF(0, 0));
        graphics.DrawString($"Week of {FirstDayOfWeek():d}", headerFont, PdfBrushes.Gray, new PointF(0, 24));

        float currentY = 50;
        var pageHeight = pdfPage.GetClientSize().Height;
        var contentLeft = 0f;

        var weekStart = FirstDayOfWeek();
        var weekEnd = weekStart.AddDays(7);

        var weekAppointments = _appointments
            .Where(appointment => appointment.StartTime < weekEnd && appointment.EndTime > weekStart)
            .OrderBy(appointment => appointment.StartTime)
            .ToList();

        // Legend of categories at top (color + label)
        var categories = weekAppointments
            .Select(appointment => appointment.Notes ?? string.Empty)
            .Distinct()
            .Where(label => !string.IsNullOrWhiteSpace(label))
            .OrderBy(label => label)
            .ToList();

        if (categories.Count > 0)
        {
            float legendX = contentLeft;
            const float legendBox = 10f;
            const float legendGap = 12f;
            foreach (var categoryLabel in categories)
            {
                var sample = weekAppointments.First(appointment => (appointment.Notes ?? string.Empty) == categoryLabel);
                var color = ExtractPdfColor(sample);
                graphics.DrawRectangle(new PdfSolidBrush(color), new RectangleF(legendX, currentY, legendBox, legendBox));
                graphics.DrawString(categoryLabel, textFont, PdfBrushes.Black, new PointF(legendX + legendBox + 4, currentY - 1));
                legendX += legendBox + 4 + (float)textFont.MeasureString(categoryLabel).Width + legendGap;
            }
            currentY += 18;
        }

        // Group by day and list items with color bullet
        var groups = weekAppointments
            .GroupBy(appointment => appointment.StartTime.Date)
            .OrderBy(grouping => grouping.Key);

        foreach (var dayGroup in groups)
        {
            // Page break if needed
            if (currentY > pageHeight - 40)
            {
                pdfPage = pdfDocument.Pages.Add();
                graphics = pdfPage.Graphics;
                currentY = 36;
            }

            var dayHeader = dayGroup.Key.ToString("dddd, MMM d");
            graphics.DrawString(dayHeader, headerFont, PdfBrushes.Black, new PointF(contentLeft, currentY));
            currentY += 16;

            foreach (var dayAppointment in dayGroup)
            {
                if (currentY > pageHeight - 24)
                {
                    pdfPage = pdfDocument.Pages.Add();
                    graphics = pdfPage.Graphics;
                    currentY = 36;
                }

                var color = ExtractPdfColor(dayAppointment);
                // Color bullet
                graphics.DrawRectangle(new PdfSolidBrush(color), new RectangleF(contentLeft, currentY + 3, 8, 8));

                var timeRange = $"{dayAppointment.StartTime:hh:mm tt} - {dayAppointment.EndTime:hh:mm tt}";
                var line = $"{timeRange}  |  {dayAppointment.Subject}";
                if (!string.IsNullOrWhiteSpace(dayAppointment.Location))
                    line += $"  @ {dayAppointment.Location}";
                if (!string.IsNullOrWhiteSpace(dayAppointment.Notes))
                    line += $"  —  {dayAppointment.Notes}";

                graphics.DrawString(line, textFont, PdfBrushes.Black, new PointF(contentLeft + 12, currentY));
                currentY += 16;
            }

            currentY += 6; // small gap after each day
        }

        using var pdfStream = new MemoryStream();
        pdfDocument.Save(pdfStream);
        pdfStream.Position = 0;

        var fileName = $"WeeklySchedule_{DateTime.Now:yyyyMMdd_HHmm}.pdf";
        var filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);
        using (var fileStream = File.Open(filePath, FileMode.Create, FileAccess.Write))
        {
            await pdfStream.CopyToAsync(fileStream);
        }

        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = "Weekly Schedule",
            File = new ShareFile(filePath)
        });
    }
    #endregion

    #region Helpers
    /// <summary>
    /// Gets the first day (Sunday) of the week for the Scheduler's current DisplayDate.
    /// </summary>
    /// <returns>First day of the current week.</returns>
    /// <summary>
    /// Calculates the first day of the week (Sunday) based on the scheduler's current display date.
    /// </summary>
    /// <returns>The date representing the week's Sunday.</returns>
    private DateTime FirstDayOfWeek()
    {
        var date = Scheduler.DisplayDate.Date;
        int diff = (7 + (date.DayOfWeek - DayOfWeek.Sunday)) % 7;
        return date.AddDays(-1 * diff).Date;
    }

    /// <summary>
    /// Attempts to read a solid color from the appointment background and converts it to a PdfColor.
    /// </summary>
    /// <param name="appointment">The appointment whose color is extracted.</param>
    /// <returns>A PdfColor representing the appointment color or a default when unavailable.</returns>
    private static PdfColor ExtractPdfColor(SchedulerAppointment appointment)
    {
        // Try to read SolidColorBrush from Background; fallback to a default
        try
        {
            if (appointment.Background is SolidColorBrush solidColorBrush)
            {
                return ToPdfColor(solidColorBrush.Color);
            }
        }
        catch { }
        return new PdfColor(93, 156, 236); // default blue
    }

    /// <summary>
    /// Converts a MAUI Color to a PdfColor.
    /// </summary>
    /// <param name="color">The MAUI color.</param>
    /// <returns>The equivalent PdfColor.</returns>
    private static PdfColor ToPdfColor(Color color)
    {
        byte redByte = (byte)(color.Red * 255);
        byte greenByte = (byte)(color.Green * 255);
        byte blueByte = (byte)(color.Blue * 255);
        byte alphaByte = (byte)(color.Alpha * 255);
        return new PdfColor(alphaByte, redByte, greenByte, blueByte);
    }
    #endregion
}
