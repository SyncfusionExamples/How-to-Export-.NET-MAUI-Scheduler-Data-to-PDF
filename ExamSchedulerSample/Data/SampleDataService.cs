using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Graphics;

namespace ExamSchedulerSample
{
    /// <summary>
    /// Provides deterministic sample/master data and utility methods for building
    /// a complete exam schedule used by the app (halls, invigilators, courses, and appointments).
    /// This is intended for demos, prototypes, and design-time data.
    /// </summary>
    public static class SampleDataService
    {
        /// <summary>
        /// Returns the list of exam halls with identifiers, names, capacity, and display colors.
        /// </summary>
        public static List<Hall> GetHalls() => new()
        {
            new Hall { Id = "H1", Name = "Main Hall A",  Capacity = 120, Color = Colors.Orange },
            new Hall { Id = "H2", Name = "Main Hall B",  Capacity = 100, Color = Colors.MediumPurple },
            new Hall { Id = "H3", Name = "Annex 101",    Capacity =  60, Color = Colors.SteelBlue },
            new Hall { Id = "H4", Name = "Seminar Block",Capacity =  80, Color = Colors.Teal }
        };

        /// <summary>
        /// Returns the list of invigilators with identifiers, names, departments, and display colors.
        /// </summary>
        public static List<Invigilator> GetInvigilators() => new()
        {
            new Invigilator { Id = "T1", Name = "Dr. Anitha", Department = "CSE", Color = Colors.SeaGreen },
            new Invigilator { Id = "T2", Name = "Mr. Kumar",  Department = "ECE", Color = Colors.Teal },
            new Invigilator { Id = "T3", Name = "Ms. Roy",    Department = "EEE", Color = Colors.IndianRed },
            new Invigilator { Id = "T4", Name = "Dr. Varun",  Department = "IT",  Color = Colors.DarkCyan }
        };

        /// <summary>
        /// Returns a curated list of courses across CSE, ECE, and EEE departments
        /// with codes, titles, and semester numbers.
        /// </summary>
        public static List<Course> GetCourses() => new()
        {
            new Course { Code = "CS101E", Title = "Data Structures",        Semester = 3, Department = "CSE" },
            new Course { Code = "CS204E", Title = "Operating Systems",      Semester = 4, Department = "CSE" },
            new Course { Code = "CS305E", Title = "Compiler Design",        Semester = 5, Department = "CSE" },
            new Course { Code = "CS312E", Title = "Database Systems",       Semester = 5, Department = "CSE" },
            new Course { Code = "CS350E", Title = "Computer Networks",      Semester = 6, Department = "CSE" },
            new Course { Code = "CS360E", Title = "Machine Learning",       Semester = 6, Department = "CSE" },

            new Course { Code = "EC202E", Title = "Signals & Systems",      Semester = 4, Department = "ECE" },
            new Course { Code = "EC310E", Title = "Embedded Systems",       Semester = 5, Department = "ECE" },
            new Course { Code = "EC315E", Title = "Digital Communication",  Semester = 5, Department = "ECE" },
            new Course { Code = "EC320E", Title = "VLSI Design",            Semester = 6, Department = "ECE" },
            new Course { Code = "EC330E", Title = "Microprocessors",        Semester = 6, Department = "ECE" },
            new Course { Code = "EC340E", Title = "Wireless Communication", Semester = 6, Department = "ECE" },

            new Course { Code = "EE201E", Title = "Circuit Theory",           Semester = 3, Department = "EEE" },
            new Course { Code = "EE305E", Title = "Power Electronics",        Semester = 5, Department = "EEE" },
            new Course { Code = "EE312E", Title = "Electrical Machines",      Semester = 5, Department = "EEE" },
            new Course { Code = "EE330E", Title = "Control Systems",          Semester = 6, Department = "EEE" },
            new Course { Code = "EE340E", Title = "High Voltage Engineering", Semester = 6, Department = "EEE" },
            new Course { Code = "EE350E", Title = "Renewable Energy Systems", Semester = 6, Department = "EEE" },
        };

        /// <summary>
        /// Computes a standard exam slot for the given <paramref name="date"/> and <paramref name="session"/>.
        /// FN → 09:30–12:30, AN → 14:00–17:00.
        /// </summary>
        /// <param name="date">The calendar date of the exam.</param>
        /// <param name="session">The session type (Forenoon or Afternoon).</param>
        /// <returns>A tuple of <c>(start, end)</c> times representing the exam duration.</returns>
        public static (DateTime start, DateTime end) Slot(DateTime date, ExamSession session)
        {
            var start = session == ExamSession.FN
                ? date.Date.AddHours(9).AddMinutes(30)
                : date.Date.AddHours(14);

            var end = start.AddHours(3);
            return (start, end);
        }

        /// <summary>
        /// Formats a time range as <c>HH:mm–HH:mm</c>.
        /// </summary>
        /// <param name="s">Start time.</param>
        /// <param name="e">End time.</param>
        /// <returns>A human-readable slot label.</returns>
        private static string FormatSlot(DateTime s, DateTime e)
            => $"{s:HH\\:mm}–{e:HH\\:mm}";

        /// <summary>
        /// Builds a complete list of exam appointments with:
        /// - A one-day gap enforced per department,
        /// - Daily-rotating hall order (ensures fair distribution),
        /// - Invigilators assigned with minimal repetition per day,
        /// - Four slots per day: two in FN and two in AN.
        /// </summary>
        /// <returns>The generated set of <see cref="ExamAppointment"/> items.</returns>
        public static List<ExamAppointment> GetExamAppointments()
        {
            var halls = GetHalls();
            var invigilators = GetInvigilators();
            var courses = GetCourses();

            var deptOrder = new[] { "CSE", "ECE", "EEE" };
            var deptQueues = deptOrder.ToDictionary(
                department => department, department => new Queue<Course>(courses.Where(course => course.Department == department)));

            var lastDayForDept = deptOrder.ToDictionary(department => department, department => DateTime.MinValue.Date);

            var appointments = new List<ExamAppointment>();
            var baseDay = DateTime.Today.AddDays(3).Date;
            var day = baseDay;
            int deptStartIndex = 0;
            int invIndexGlobal = 0;

            bool TryGetNextEligibleDept(DateTime targetDay, int startIdx, out string dept)
            {
                for (int i = 0; i < deptOrder.Length; i++)
                {
                    var department = deptOrder[(startIdx + i) % deptOrder.Length];
                    if (deptQueues[department].Count == 0) continue;

                    if (lastDayForDept[department] <= targetDay.AddDays(-1))
                    {
                        dept = department;
                        return true;
                    }
                }
                dept = "";
                return false;
            }

            while (deptQueues.Values.Any(q => q.Count > 0))
            {
                int dayIndex = (int)(day - baseDay).TotalDays;

                var hallsRotated = halls
                    .Skip(dayIndex % halls.Count)
                    .Concat(halls.Take(dayIndex % halls.Count))
                    .ToList();

                var dayPlan = new (ExamSession session, Hall hall)[]
                {
                    (ExamSession.FN, hallsRotated[0]),
                    (ExamSession.FN, hallsRotated[1]),
                    (ExamSession.AN, hallsRotated[2]),
                    (ExamSession.AN, hallsRotated[3])
                };

                var usedInvigilatorsToday = new HashSet<string>();

                foreach (var slot in dayPlan)
                {
                    if (!deptQueues.Values.Any(queue => queue.Count > 0))
                        break;

                    if (!TryGetNextEligibleDept(day, deptStartIndex, out var dept))
                        continue;

                    var course = deptQueues[dept].Dequeue();

                    Invigilator inv = null!;
                    int tries = 0;
                    while (tries < invigilators.Count)
                    {
                        inv = invigilators[invIndexGlobal % invigilators.Count];
                        invIndexGlobal++;
                        if (!usedInvigilatorsToday.Contains(inv.Id))
                            break;
                        tries++;
                    }
                    usedInvigilatorsToday.Add(inv.Id);

                    var (start, end) = Slot(day, slot.session);
                    var notes = $"{FormatSlot(start, end)}  •  Room: {slot.hall.Name}  •  Invigilator: {inv.Name}";

                    var appointment = new ExamAppointment
                    {
                        Start = start,
                        End = end,
                        Subject = $"{course.Code} – {course.Title}",
                        Notes = notes,
                        HallId = slot.hall.Id,
                        HallName = slot.hall.Name,
                        InvigilatorId = inv.Id,
                        InvigilatorName = inv.Name,
                        Background = slot.hall.Color
                    };

                    appointment.ResourceIds.Add(slot.hall.Id);
                    appointments.Add(appointment);

                    lastDayForDept[dept] = day;
                    deptStartIndex = (Array.IndexOf(deptOrder, dept) + 1) % deptOrder.Length;
                }

                day = day.AddDays(1);
            }

            return appointments;
        }
    }
}
