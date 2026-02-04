using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using PointF = Syncfusion.Drawing.PointF;
using RectangleF = Syncfusion.Drawing.RectangleF;
using SizeF = Syncfusion.Drawing.SizeF;

namespace ExamSchedulerSample.Services
{
    /// <summary>
    /// Provides PDF export and preview services for exam schedule data, including timetable, invigilator duties, and hall availability. Supports program-specific (UG/PG/MBA) exports.
    /// </summary>
    public static class PdfExportService
    {
        /// <summary>
        /// Public helper to build the PDF bytes for a given program (UG/PG/MBA). Exposes the internal PDF creation to UI code.
        /// </summary>
        public static byte[] GetPdfBytes(ProgramType program) => CreatePdfBytes(program);

        /// <summary>
        /// Builds the exam schedule PDF for the default program (UG) and invokes the platform save/view service.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task ExportAllAsync() => ExportAllAsync(ProgramType.UG);

        /// <summary>
        /// Builds the exam schedule PDF for the specified program and invokes the platform save/view service.
        /// </summary>
        /// <param name="program">The program to export (UG/PG/MBA).</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task ExportAllAsync(ProgramType program)
        {
            var pdfBytes = await Task.Run(() => CreatePdfBytes(program));

            using var stream = new MemoryStream(pdfBytes, writable: false);
            var fileName = $"ExamSchedule-{program}.pdf";
            await SaveService.SaveAndViewAsync(fileName, "application/pdf", stream);
        }

        /// <summary>
        /// Builds the exam schedule PDF for the default program (UG) and navigates to a preview page that can handle printing/sharing.
        /// </summary>
        /// <param name="navigation">The MAUI navigation instance used to push the preview page.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static Task PreviewAndPrintAsync(INavigation navigation) => PreviewAndPrintAsync(navigation, ProgramType.UG);

        /// <summary>
        /// Builds the exam schedule PDF for the specified program and navigates to a preview page that can handle printing/sharing.
        /// </summary>
        /// <param name="navigation">The MAUI navigation instance used to push the preview page.</param>
        /// <param name="program">The program to export/preview (UG/PG/MBA).</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task PreviewAndPrintAsync(INavigation navigation, ProgramType program)
        {
            var pdfBytes = await Task.Run(() => CreatePdfBytes(program));

            await MainThread.InvokeOnMainThreadAsync(() => navigation.PushAsync(new PdfPreviewPage(new MemoryStream(pdfBytes))));
        }

        /// <summary>
        /// Creates the PDF document bytes for the complete exam export for the specified program, including a cover and three sections: timetable, duties, and hall availability.
        /// </summary>
        /// <param name="program">The program to export (UG/PG/MBA).</param>
        /// <returns>The PDF document as a byte array.</returns>
        private static byte[] CreatePdfBytes(ProgramType program)
        {
            var halls = SampleDataPage.GetHalls();
            var invigilators = SampleDataPage.GetInvigilators();
            var appointments = SampleDataPage.GetExamAppointments(program);

            var invigilatorDutyRows = BuildInvigilatorDutyRows(appointments, halls, invigilators);
            var hallAvailabilityRows = BuildHallAvailabilityRows(appointments, halls);
            var timetableRows = BuildTimetableRowsData(appointments);

            using var document = new PdfDocument
            {
                PageSettings = { Size = PdfPageSize.A4 }
            };

            // Cover
            var coverPage = document.Pages.Add();
            DrawCover(coverPage.Graphics, $"University Exam Schedule — {program}");

            // Section 1: Timetable
            var timetablePage = document.Pages.Add();
            float afterHeaderY = DrawSectionHeader(timetablePage.Graphics, "1) Date-wise Exam Timetable");
            DrawGridWithPagination(
                timetablePage,
                timetableRows,
                new[] { "Date", "Session", "Course", "Hall", "Invigilator" },
                afterHeaderY + 8f);

            // Section 2: Invigilator Duty Roster
            var dutiesPage = document.Pages.Add();
            afterHeaderY = DrawSectionHeader(dutiesPage.Graphics, "2) Invigilator Duty Roster");
            DrawGridWithPagination(
                dutiesPage,
                invigilatorDutyRows,
                new[] { "Date", "Session", "Invigilator", "Hall", "Course" },
                afterHeaderY + 8f);

            // Section 3: Hall Availability
            var availabilityPage = document.Pages.Add();
            afterHeaderY = DrawSectionHeader(availabilityPage.Graphics, "3) Exam Hall Availability");
            DrawGridWithPagination(
                availabilityPage,
                hallAvailabilityRows,
                new[] { "Date", "Session", "Hall", "Capacity", "Status" },
                afterHeaderY + 8f);

            using var memoryStream = new MemoryStream();
            document.Save(memoryStream);
            return memoryStream.ToArray();
        }

        /// <summary>
        /// Draws the cover page with a banner, a custom title (e.g., with program), generation timestamp, and a simple contents list.
        /// </summary>
        /// <param name="graphics">The graphics context of the current PDF page.</param>
        /// <param name="title">Custom title text to render on the cover.</param>
        private static void DrawCover(PdfGraphics graphics, string title)
        {
            const float margin = 48f;
            SizeF clientSize = graphics.ClientSize;
            float left = margin;
            float top = margin;
            float right = clientSize.Width - margin;
            float width = right - left;

            const float bannerHeight = 88f;
            var bannerRect = new RectangleF(left, top, width, bannerHeight);
            graphics.DrawRectangle(PdfBrushes.LightSteelBlue, bannerRect);
            graphics.DrawRectangle(PdfPens.SlateGray, bannerRect);

            var titleFont = new PdfStandardFont(PdfFontFamily.Helvetica, 22, PdfFontStyle.Bold);
            var subtitleFont = new PdfStandardFont(PdfFontFamily.Helvetica, 12);
            graphics.DrawString(title, titleFont, PdfBrushes.DarkBlue, new PointF(left + 14, top + 24));
            graphics.DrawString($"Generated: {DateTime.Now:dd-MMM-yyyy HH:mm}", subtitleFont, PdfBrushes.Gray, new PointF(left + 14, top + 56));

            float y = top + bannerHeight + 28f;
            var headingFont = new PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold);
            var paragraphFont = new PdfStandardFont(PdfFontFamily.Helvetica, 12);

            graphics.DrawString("Contents", headingFont, PdfBrushes.Black, new PointF(left, y));
            y += 16;
            graphics.DrawString("• Date-wise Exam Timetable", paragraphFont, PdfBrushes.Black, new PointF(left, y)); y += 14;
            graphics.DrawString("• Invigilator Duty Roster", paragraphFont, PdfBrushes.Black, new PointF(left, y)); y += 14;
            graphics.DrawString("• Exam Hall Availability", paragraphFont, PdfBrushes.Black, new PointF(left, y));
        }

        /// <summary>
        /// Draws a bold section header at standard margins and returns the bottom Y coordinate of the header block.
        /// </summary>
        /// <param name="graphics">The graphics context of the current PDF page.</param>
        /// <param name="text">The header text to render.</param>
        /// <returns>The Y coordinate immediately below the header, suitable for content placement.</returns>
        private static float DrawSectionHeader(PdfGraphics graphics, string text)
        {
            const float margin = 48f;
            float x = margin;
            float y = margin;

            var headerFont = new PdfStandardFont(PdfFontFamily.Helvetica, 16, PdfFontStyle.Bold);
            graphics.DrawString(text, headerFont, PdfBrushes.Black, new PointF(x, y));
            y += 18;
            graphics.DrawLine(PdfPens.Gray, new PointF(x, y + 4), new PointF(graphics.ClientSize.Width - margin, y + 4));
            return y + 6;
        }

        /// <summary>
        /// Draws a <see cref="PdfGrid"/> with pagination support starting at (margin, <paramref name="startY"/>). Column headers are supplied explicitly via <paramref name="headers"/>.
        /// </summary>
        private static void DrawGridWithPagination<T>(PdfPage page, IEnumerable<T> rows, string[] headers, float startY)
        {
            const float margin = 48f;

            var grid = new PdfGrid
            {
                Style = { CellPadding = new PdfPaddings(4, 4, 4, 4) }
            };

            var rowList = rows?.ToList() ?? new List<T>();
            grid.DataSource = rowList;

            if (grid.Headers.Count == 0)
            {
                grid.Headers.Add(1);
            }

            var headerRow = grid.Headers[0];
            for (int i = 0; i < Math.Min(headerRow.Cells.Count, headers.Length); i++)
            {
                headerRow.Cells[i].Value = headers[i];
            }

            var layout = new PdfGridLayoutFormat
            {
                Break = PdfLayoutBreakType.FitPage,
                Layout = PdfLayoutType.Paginate
            };

            grid.Draw(page, new PointF(margin, startY), layout);
        }

        /// <summary>
        /// Builds a flat, grid-ready set of timetable rows (date/session/course/hall/invigilator).
        /// </summary>
        private static IEnumerable<object> BuildTimetableRowsData(List<ExamAppointment> appointments)
        {
            return (appointments ?? new List<ExamAppointment>())
                .OrderBy(appointment => appointment.Start)
                .Select(appointment => new
                {
                    Date = appointment.Start.Date.ToString("dd-MMM-yyyy"),
                    Session = appointment.Start.Hour < 13 ? "FN" : "AN",
                    Course = appointment.Subject,
                    Hall = appointment.HallName,
                    Invigilator = appointment.InvigilatorName
                });
        }

        /// <summary>
        /// Builds the invigilator duty allocation rows from appointments, halls, and invigilators.
        /// </summary>
        private static List<InvigilatorDutyRow> BuildInvigilatorDutyRows(
            List<ExamAppointment> appointments,
            List<Hall> halls,
            List<Invigilator> invigilators)
        {
            var hallById = (halls ?? new List<Hall>()).ToDictionary(hall => hall.Id, hall => hall.Name);
            var invigilatorById = (invigilators ?? new List<Invigilator>()).ToDictionary(invigilator => invigilator.Id, invigilator => invigilator.Name);

            var rows = new List<InvigilatorDutyRow>();
            foreach (var appointment in (appointments ?? new List<ExamAppointment>()).OrderBy(appointment => appointment.Start))
            {
                rows.Add(new InvigilatorDutyRow
                {
                    Date = appointment.Start.Date,
                    Session = appointment.Start.Hour < 13 ? ExamSession.FN : ExamSession.AN,
                    Invigilator = invigilatorById.TryGetValue(appointment.InvigilatorId, out var invName) ? invName : (appointment.InvigilatorName ?? appointment.InvigilatorId),
                    Hall = hallById.TryGetValue(appointment.HallId, out var hallName) ? hallName : (appointment.HallName ?? appointment.HallId),
                    Course = appointment.Subject
                });
            }
            return rows;
        }

        /// <summary>
        /// Builds the hall availability rows (per date/session/hall) based on the scheduled appointments.
        /// </summary>
        private static List<HallAvailabilityRow> BuildHallAvailabilityRows(List<ExamAppointment> appointments, List<Hall> halls)
        {
            var resultRows = new List<HallAvailabilityRow>();
            var appointmentList = appointments ?? new List<ExamAppointment>();
            var hallList = halls ?? new List<Hall>();

            var dates = appointmentList
                            .Select(appointment => appointment.Start.Date)
                            .Distinct()
                            .OrderBy(date => date);

            foreach (var date in dates)
            {
                foreach (var hall in hallList)
                {
                    foreach (var session in new[] { ExamSession.FN, ExamSession.AN })
                    {
                        var slotStart = session == ExamSession.FN ? date.AddHours(9).AddMinutes(30) : date.AddHours(14);
                        var isAllocated = appointmentList.Any(appointment => appointment.HallId == hall.Id && appointment.Start == slotStart);

                        resultRows.Add(new HallAvailabilityRow
                        {
                            Date = date,
                            Session = session,
                            Hall = hall.Name,
                            Capacity = hall.Capacity,
                            Status = isAllocated ? "Allocated" : "Available"
                        });
                    }
                }
            }

            return resultRows;
        }
    }
}