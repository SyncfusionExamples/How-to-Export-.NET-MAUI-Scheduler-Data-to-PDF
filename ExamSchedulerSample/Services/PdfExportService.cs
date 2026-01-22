
// File: Services/PdfExportService.cs
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;

// Type aliases for Syncfusion.Drawing primitives
using PointF = Syncfusion.Drawing.PointF;
using RectangleF = Syncfusion.Drawing.RectangleF;
using SizeF = Syncfusion.Drawing.SizeF;

namespace ExamSchedulerSample.Services
{
    public static class PdfExportService
    {
        // --------------------------------------------------------------------
        // Public entry points
        // --------------------------------------------------------------------
        public static async Task ExportAllAsync()
        {
            var bytes = BuildPdfBytes(); // single source of truth for both Export & Preview
            using var ms = new MemoryStream(bytes);
            ms.Position = 0;
            await SaveService.SaveAndViewAsync("ExamSchedule.pdf", "application/pdf", ms);
        }

        public static async Task PreviewAndPrintAsync(INavigation nav)
        {
            var bytes = BuildPdfBytes(); // same content as Export
            // Pass a fresh MemoryStream; the preview page will take ownership/copy defensively
            await nav.PushAsync(new PdfPreviewPage(new MemoryStream(bytes)));
        }

        // --------------------------------------------------------------------
        // PDF builder (shared)
        // --------------------------------------------------------------------
        private static byte[] BuildPdfBytes()
        {
            // Collect data
            var halls = SampleDataService.GetHalls();
            var invs = SampleDataService.GetInvigilators();
            var appts = SampleDataService.GetExamAppointments();

            // Derived tables
            var dutyRows = BuildInvigilatorDuties(appts, halls, invs);
            var availabilityRows = BuildHallAvailability(appts, halls);
            var timetableRows = BuildTimetableRows(appts); // uses InvigilatorName directly

            using var doc = new PdfDocument
            {
                PageSettings = { Size = PdfPageSize.A4 } // explicit size for predictability
            };

            // ---- Cover page ----
            var cover = doc.Pages.Add();
            DrawCover(cover.Graphics);

            // ---- Section 1: Date-wise Exam Timetable ----
            var sec1 = doc.Pages.Add();
            float afterHeaderY = DrawSectionHeader(sec1.Graphics, "1) Date-wise Exam Timetable");
            DrawGrid(sec1, timetableRows,
                new[] { "Date", "Session", "Course", "Hall", "Invigilator" },
                afterHeaderY + 8);

            // ---- Section 2: Invigilator Duty Roster ----
            var sec2 = doc.Pages.Add();
            afterHeaderY = DrawSectionHeader(sec2.Graphics, "2) Invigilator Duty Roster");
            DrawGrid(sec2, dutyRows,
                new[] { "Date", "Session", "Invigilator", "Hall", "Course" },
                afterHeaderY + 8);

            // ---- Section 3: Hall Availability ----
            var sec3 = doc.Pages.Add();
            afterHeaderY = DrawSectionHeader(sec3.Graphics, "3) Exam Hall Availability");
            DrawGrid(sec3, availabilityRows,
                new[] { "Date", "Session", "Hall", "Capacity", "Status" },
                afterHeaderY + 8);

            // Save to bytes
            using var ms = new MemoryStream();
            doc.Save(ms);
            return ms.ToArray();
        }

        // --------------------------------------------------------------------
        // Drawing helpers
        // --------------------------------------------------------------------
        private static void DrawCover(PdfGraphics g)
        {
            // Margins and client size
            float margin = 48f; // ~0.67 inch
            SizeF client = g.ClientSize;
            float left = margin;
            float top = margin;
            float right = client.Width - margin;
            float width = right - left;

            // Banner rectangle (ensures page never looks visually empty)
            float bannerHeight = 88f;
            var bannerRect = new RectangleF(left, top, width, bannerHeight);
            g.DrawRectangle(PdfBrushes.LightSteelBlue, bannerRect);
            g.DrawRectangle(PdfPens.SlateGray, bannerRect);

            // Title + timestamp inside banner
            var titleFont = new PdfStandardFont(PdfFontFamily.Helvetica, 22, PdfFontStyle.Bold);
            var subFont = new PdfStandardFont(PdfFontFamily.Helvetica, 12);
            g.DrawString("University Exam Schedule", titleFont, PdfBrushes.DarkBlue, new PointF(left + 14, top + 24));
            g.DrawString($"Generated: {DateTime.Now:dd-MMM-yyyy HH:mm}", subFont, PdfBrushes.Gray, new PointF(left + 14, top + 56));

            // Contents list
            float y = top + bannerHeight + 28f;
            var h2 = new PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold);
            var p = new PdfStandardFont(PdfFontFamily.Helvetica, 12);

            g.DrawString("Contents", h2, PdfBrushes.Black, new PointF(left, y));
            y += 16;
            g.DrawString("• Date-wise Exam Timetable", p, PdfBrushes.Black, new PointF(left, y)); y += 14;
            g.DrawString("• Invigilator Duty Roster", p, PdfBrushes.Black, new PointF(left, y)); y += 14;
            g.DrawString("• Exam Hall Availability", p, PdfBrushes.Black, new PointF(left, y));
        }

        /// <summary>
        /// Draw a bold section header at standard margins; returns the bottom Y of the header block.
        /// </summary>
        private static float DrawSectionHeader(PdfGraphics g, string text)
        {
            float margin = 48f;
            float x = margin;
            float y = margin;

            var hFont = new PdfStandardFont(PdfFontFamily.Helvetica, 16, PdfFontStyle.Bold);
            g.DrawString(text, hFont, PdfBrushes.Black, new PointF(x, y));
            y += 18;
            g.DrawLine(PdfPens.Gray, new PointF(x, y + 4), new PointF(g.ClientSize.Width - margin, y + 4));
            return y + 6;
        }

        /// <summary>
        /// Draw a PdfGrid with paginate layout, starting at (margin, startY).
        /// </summary>
        private static void DrawGrid<T>(PdfPage page, IEnumerable<T> rows, string[] headers, float startY)
        {
            float margin = 48f;

            var grid = new PdfGrid
            {
                Style = { CellPadding = new PdfPaddings(4, 4, 4, 4) }
            };

            // Bind data
            var list = rows?.ToList() ?? new List<T>();
            grid.DataSource = list;

            // Ensure there is a header row and set captions
            if (grid.Headers.Count == 0)
            {
                grid.Headers.Add(1);
            }

            var header = grid.Headers[0];
            for (int i = 0; i < Math.Min(header.Cells.Count, headers.Length); i++)
            {
                header.Cells[i].Value = headers[i];
            }

            // Paginate if content flows beyond page
            var layout = new PdfGridLayoutFormat
            {
                Break = PdfLayoutBreakType.FitPage,
                Layout = PdfLayoutType.Paginate
            };

            grid.Draw(page, new PointF(margin, startY), layout);
        }

        // --------------------------------------------------------------------
        // Row builders
        // --------------------------------------------------------------------
        private static IEnumerable<object> BuildTimetableRows(List<ExamAppointment> appts)
        {
            return appts
                .OrderBy(a => a.Start)
                .Select(a => new
                {
                    Date = a.Start.Date.ToString("dd-MMM-yyyy"),
                    Session = a.Start.Hour < 13 ? "FN" : "AN",
                    Course = a.Subject,
                    Hall = a.HallName,
                    Invigilator = a.InvigilatorName // ensure name prints, not ID
                });
        }

        private static List<InvigilatorDutyRow> BuildInvigilatorDuties(
            List<ExamAppointment> appts,
            List<Hall> halls,
            List<Invigilator> invs)
        {
            var hallMap = halls.ToDictionary(h => h.Id, h => h.Name);
            var invMap = invs.ToDictionary(t => t.Id, t => t.Name);

            var rows = new List<InvigilatorDutyRow>();
            foreach (var a in appts.OrderBy(a => a.Start))
            {
                rows.Add(new InvigilatorDutyRow
                {
                    Date = a.Start.Date,
                    Session = a.Start.Hour < 13 ? ExamSession.FN : ExamSession.AN,
                    Invigilator = invMap.TryGetValue(a.InvigilatorId, out var n) ? n : (a.InvigilatorName ?? a.InvigilatorId),
                    Hall = hallMap.TryGetValue(a.HallId, out var hn) ? hn : (a.HallName ?? a.HallId),
                    Course = a.Subject
                });
            }
            return rows;
        }

        private static List<HallAvailabilityRow> BuildHallAvailability(List<ExamAppointment> appts, List<Hall> halls)
        {
            // For demo: mark hall as "Allocated" if any appointment exists in FN/AN
            var rows = new List<HallAvailabilityRow>();
            var dates = appts.Select(a => a.Start.Date).Distinct().OrderBy(d => d);

            foreach (var d in dates)
            {
                foreach (var hall in halls)
                {
                    foreach (var ses in new[] { ExamSession.FN, ExamSession.AN })
                    {
                        var slotStart = ses == ExamSession.FN ? d.AddHours(9).AddMinutes(30) : d.AddHours(14);
                        var any = appts.Any(a => a.HallId == hall.Id && a.Start == slotStart);

                        rows.Add(new HallAvailabilityRow
                        {
                            Date = d,
                            Session = ses,
                            Hall = hall.Name,
                            Capacity = hall.Capacity,
                            Status = any ? "Allocated" : "Available"
                        });
                    }
                }
            }

            return rows;
        }
    }
}
