
// File: SampleDataService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Graphics;

namespace ExamSchedulerSample
{
    public static class SampleDataService
    {
        // -----------------------------
        // Master data
        // -----------------------------
        public static List<Hall> GetHalls() => new()
        {
            new Hall { Id = "H1", Name = "Main Hall A",  Capacity = 120, Color = Colors.Orange },
            new Hall { Id = "H2", Name = "Main Hall B",  Capacity = 100, Color = Colors.MediumPurple },
            new Hall { Id = "H3", Name = "Annex 101",    Capacity =  60, Color = Colors.SteelBlue }
        };

        public static List<Invigilator> GetInvigilators() => new()
        {
            new Invigilator { Id = "T1", Name = "Dr. Anitha", Department = "CSE", Color = Colors.SeaGreen },
            new Invigilator { Id = "T2", Name = "Mr. Kumar",  Department = "ECE", Color = Colors.Teal },
            new Invigilator { Id = "T3", Name = "Ms. Roy",    Department = "EEE", Color = Colors.IndianRed },
            new Invigilator { Id = "T4", Name = "Dr. Varun",  Department = "IT",  Color = Colors.DarkCyan }
        };

        // Departments: CSE, ECE, EEE (no Maths). 4 subjects per department.
        public static List<Course> GetCourses() => new()
        {
            // CSE (4)
            new Course { Code = "CS101", Title = "Data Structures",        Semester = 3, Department = "CSE" },
            new Course { Code = "CS204", Title = "Operating Systems",      Semester = 4, Department = "CSE" },
            new Course { Code = "CS305", Title = "Compiler Design",        Semester = 5, Department = "CSE" },
            new Course { Code = "CS312", Title = "Database Systems",       Semester = 5, Department = "CSE" },

            // ECE (4)
            new Course { Code = "EC202", Title = "Signals & Systems",      Semester = 4, Department = "ECE" },
            new Course { Code = "EC310", Title = "Embedded Systems",       Semester = 5, Department = "ECE" },
            new Course { Code = "EC315", Title = "Digital Communication",  Semester = 5, Department = "ECE" },
            new Course { Code = "EC320", Title = "VLSI Design",            Semester = 6, Department = "ECE" },

            // EEE (4)
            new Course { Code = "EE201", Title = "Circuit Theory",         Semester = 3, Department = "EEE" },
            new Course { Code = "EE305", Title = "Power Electronics",      Semester = 5, Department = "EEE" },
            new Course { Code = "EE312", Title = "Electrical Machines",    Semester = 5, Department = "EEE" },
            new Course { Code = "EE330", Title = "Control Systems",        Semester = 6, Department = "EEE" },
        };

        // -----------------------------
        // Helpers
        // -----------------------------
        /// <summary>
        /// Typical university slots: FN 09:30–12:30, AN 14:00–17:00
        /// </summary>
        public static (DateTime start, DateTime end) Slot(DateTime date, ExamSession session)
        {
            var start = session == ExamSession.FN
                ? date.Date.AddHours(9).AddMinutes(30)
                : date.Date.AddHours(14);
            var end = start.AddHours(3);
            return (start, end);
        }

        private static string FormatSlot(DateTime s, DateTime e)
            => $"{s:HH\\:mm}–{e:HH\\:mm}";

        // -----------------------------
        // Appointments
        // -----------------------------
        /// <summary>
        /// Builds a timetable where:
        /// - Each department has a 1-day gap between its exams (no back-to-back days).
        /// - Other departments can occupy the in-between days.
        /// - Invigilators rotate globally across appointments; within a day we avoid repeats if possible.
        /// - Uses 4 slots per day: FN(H1, H2), AN(H3, H4).
        /// </summary>
        public static List<ExamAppointment> GetExamAppointments()
        {
            var halls = GetHalls();        // H1..H4
            var invs = GetInvigilators(); // T1..T4
            var courses = GetCourses();      // 12 courses

            // Per-department queues for enforcing 1-day gaps
            var deptOrder = new[] { "CSE", "ECE", "EEE" };
            var deptQueues = deptOrder.ToDictionary(
                d => d, d => new Queue<Course>(courses.Where(c => c.Department == d)));

            // Last scheduled day for each department
            var lastDayForDept = deptOrder.ToDictionary(d => d, d => DateTime.MinValue.Date);

            var appts = new List<ExamAppointment>();
            var baseDay = DateTime.Today.AddDays(4).Date;
            var day = baseDay;

            int deptStartIndex = 0;

            // NEW: global rotation index and per-day used set
            int invIndexGlobal = 0;

            bool TryGetNextEligibleDept(DateTime targetDay, int startIdx, out string chosenDept)
            {
                for (int i = 0; i < deptOrder.Length; i++)
                {
                    var d = deptOrder[(startIdx + i) % deptOrder.Length];
                    if (deptQueues[d].Count == 0) continue;

                    var last = lastDayForDept[d];
                    // Enforce 1-day gap
                    if (last <= targetDay.AddDays(-1))
                    {
                        chosenDept = d;
                        return true;
                    }
                }
                chosenDept = "";
                return false;
            }

            while (deptQueues.Values.Any(q => q.Count > 0))
            {
                // One day layout: FN => H1/H2, AN => H3/H4
                var dayPlan = new (ExamSession session, Hall hall)[]
                {
                    (ExamSession.FN, halls[0]),
                    (ExamSession.FN, halls[1]),
                    (ExamSession.AN, halls[2]),
                    (ExamSession.AN, halls[3]),
                };

                // Track invigilators already used this day (avoid repeating if possible)
                var usedInvigilatorsToday = new HashSet<string>();

                foreach (var slot in dayPlan)
                {
                    if (!deptQueues.Values.Any(q => q.Count > 0))
                        break;

                    if (!TryGetNextEligibleDept(day, deptStartIndex, out var dept))
                        continue;

                    var course = deptQueues[dept].Dequeue();

                    // ---- NEW: pick next invigilator globally; avoid repeats within the day if we can ----
                    Invigilator inv = null!;
                    int tries = 0;
                    // Try up to invs.Count times to find an invigilator not used yet today
                    while (tries < invs.Count)
                    {
                        inv = invs[invIndexGlobal % invs.Count];
                        invIndexGlobal++; // advance global index regardless so the sequence continues across days
                        if (!usedInvigilatorsToday.Contains(inv.Id))
                            break;
                        tries++;
                    }
                    // Mark used for today
                    usedInvigilatorsToday.Add(inv.Id);

                    var (s, e) = Slot(day, slot.session);

                    // Compose Notes to include slot + room + invigilator
                    var notesLine = $"{FormatSlot(s, e)}  •  Room: {slot.hall.Name}  •  Invigilator: {inv.Name}";

                    var appt = new ExamAppointment
                    {
                        Start = s,
                        End = e,
                        Subject = $"{course.Code} – {course.Title}",

                        // This 'Notes' line is what you show on the second line in XAML
                        Notes = notesLine,

                        // Also store individually (useful for exports/other views)
                        HallId = slot.hall.Id,
                        HallName = slot.hall.Name,
                        InvigilatorId = inv.Id,
                        InvigilatorName = inv.Name,

                        // Color this appointment using the hall's color
                        Background = slot.hall.Color
                    };

                    // Resource grouping by Hall
                    appt.ResourceIds.Add(slot.hall.Id);
                    appts.Add(appt);

                    // Update last day & rotate the starting dept for the next slot
                    lastDayForDept[dept] = day;
                    deptStartIndex = (Array.IndexOf(deptOrder, dept) + 1) % deptOrder.Length;
                }

                // Next calendar day
                day = day.AddDays(1);
            }

            return appts;
        }
    }
}
