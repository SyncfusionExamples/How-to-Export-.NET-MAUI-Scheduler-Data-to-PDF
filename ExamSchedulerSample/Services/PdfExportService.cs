
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using Syncfusion.Drawing;
using PointF = Syncfusion.Drawing.PointF;

namespace ExamSchedulerSample.Services
{
    public static class PdfExportService
    {
        public static async Task ExportAllAsync()
        {
            // Collect data
            var halls = SampleDataService.GetHalls();
            var invs = SampleDataService.GetInvigilators();
            var appts = SampleDataService.GetExamAppointments();

            // Build derivative rows for our tables
            var dutyRows = BuildInvigilatorDuties(appts, halls, invs);
            var availabilityRows = BuildHallAvailability(appts, halls);

            // Create PDF
            using var doc = new PdfDocument();

            // Cover
            var cover = doc.Pages.Add();
            var g = cover.Graphics;
            var titleFont = new PdfStandardFont(PdfFontFamily.Helvetica, 20, PdfFontStyle.Bold);
            var subFont = new PdfStandardFont(PdfFontFamily.Helvetica, 12);
            g.DrawString("University Exam Schedule", titleFont, PdfBrushes.DarkBlue, new PointF(0, 40));
            g.DrawString($"Generated: {DateTime.Now:dd-MMM-yyyy HH:mm}", subFont, PdfBrushes.Gray, new PointF(0, 70));

            // Section 1: Date-wise Exam Timetable
            var sec1 = doc.Pages.Add();
            DrawSectionHeader(sec1.Graphics, "1) Date-wise Exam Timetable");
            var timetableRows = BuildTimetableRows(appts, halls);
            DrawGrid(sec1, timetableRows, new[] { "Date", "Session", "Course", "Hall", "Invigilator" });

            // Section 2: Invigilator Duty Roster
            var sec2 = doc.Pages.Add();
            DrawSectionHeader(sec2.Graphics, "2) Invigilator Duty Roster");
            DrawGrid(sec2, dutyRows, new[] { "Date", "Session", "Invigilator", "Hall", "Course" });

            // Section 3: Hall Availability
            var sec3 = doc.Pages.Add();
            DrawSectionHeader(sec3.Graphics, "3) Exam Hall Availability");
            DrawGrid(sec3, availabilityRows, new[] { "Date", "Session", "Hall", "Capacity", "Status" });

            // Save & open
            using var ms = new MemoryStream();
            doc.Save(ms);
            ms.Position = 0;

            await SaveService.SaveAndViewAsync("ExamSchedule.pdf", "application/pdf", ms);
        }

        public static async Task PreviewAndPrintAsync(INavigation nav)
        {
            // Generate (re)use the last export or generate a fresh one
            using var ms = new MemoryStream();
            {
                using var tmp = new PdfDocument();
                var pg = tmp.Pages.Add();
                pg.Graphics.DrawString("Exam Schedule (Preview)", new PdfStandardFont(PdfFontFamily.Helvetica, 16), PdfBrushes.Black, new PointF(0, 0));
                tmp.Save(ms);
            }
            ms.Position = 0;

            // (Optional) Show with Syncfusion PDF Viewer and allow printing
            var page = new Pages.PdfPreviewPage(ms);
            await nav.PushAsync(page);
        }

        // ---------- Helpers ----------

        private static void DrawSectionHeader(PdfGraphics g, string text)
        {
            var f = new PdfStandardFont(PdfFontFamily.Helvetica, 16, PdfFontStyle.Bold);
            g.DrawString(text, f, PdfBrushes.Black, new PointF(0, 0));
        }

        private static void DrawGrid<T>(PdfPage page, IEnumerable<T> rows, string[] headers)
        {
            var grid = new PdfGrid();
            grid.Style.CellPadding = new PdfPaddings(4, 4, 4, 4);

            // Auto columns from object properties; set header manually
            grid.DataSource = rows.ToList();
            if (grid.Headers.Count > 0)
            {
                var header = grid.Headers[0];
                for (int i = 0; i < Math.Min(header.Cells.Count, headers.Length); i++)
                    header.Cells[i].Value = headers[i];
            }

            var layout = new PdfLayoutFormat { Break = PdfLayoutBreakType.FitPage, Layout = PdfLayoutType.Paginate };
            grid.Draw(page, new Syncfusion.Drawing.PointF(0, 30), layout);
        }

        private static IEnumerable<object> BuildTimetableRows(List<ExamAppointment> appts, List<Hall> halls)
        {
            var hallsDict = halls.ToDictionary(h => h.Id, h => h.Name);
            return appts
                .OrderBy(a => a.Start)
                .Select(a => new {
                    Date = a.Start.Date.ToString("dd-MMM-yyyy"),
                    Session = a.Start.Hour < 13 ? "FN" : "AN",
                    Course = a.Subject,
                    Hall = hallsDict.TryGetValue(a.HallId, out var name) ? name : a.HallId,
                    Invigilator = a.InvigilatorId
                });
        }

        private static List<InvigilatorDutyRow> BuildInvigilatorDuties(List<ExamAppointment> appts, List<Hall> halls, List<Invigilator> invs)
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
                    Invigilator = invMap.TryGetValue(a.InvigilatorId, out var n) ? n : a.InvigilatorId,
                    Hall = hallMap.TryGetValue(a.HallId, out var hn) ? hn : a.HallId,
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
