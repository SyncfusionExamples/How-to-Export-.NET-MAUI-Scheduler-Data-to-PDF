
using System;
using System.Collections.Generic;
using Microsoft.Maui.Graphics;

namespace ExamSchedulerSample
{
    public static class SampleDataService
    {
        public static List<Hall> GetHalls() => new()
        {
            new Hall{ Id="H1", Name="Main Hall A", Capacity=120, Color=Colors.Orange },
            new Hall{ Id="H2", Name="Main Hall B", Capacity=100, Color=Colors.MediumPurple },
            new Hall{ Id="H3", Name="Annex 101",  Capacity= 60, Color=Colors.SteelBlue },
        };

        public static List<Invigilator> GetInvigilators() => new()
        {
            new Invigilator{ Id="T1", Name="Dr. Anitha", Department="CSE", Color=Colors.SeaGreen },
            new Invigilator{ Id="T2", Name="Mr. Kumar",  Department="ECE", Color=Colors.Teal },
            new Invigilator{ Id="T3", Name="Ms. Roy",    Department="Maths", Color=Colors.IndianRed },
        };

        public static List<Course> GetCourses() => new()
        {
            new Course{ Code="CS101", Title="Data Structures", Semester=3, Department="CSE" },
            new Course{ Code="EC202", Title="Signals & Systems", Semester=4, Department="ECE" },
            new Course{ Code="MA301", Title="Numerical Methods", Semester=5, Department="Maths" },
        };

        public static (DateTime start, DateTime end) Slot(DateTime date, ExamSession session)
        {
            // Typical university slots: FN 09:30–12:30, AN 14:00–17:00
            var start = session == ExamSession.FN
                ? date.Date.AddHours(9).AddMinutes(30)
                : date.Date.AddHours(14);
            var end = start.AddHours(3);
            return (start, end);
        }

        public static List<ExamAppointment> GetExamAppointments()
        {
            var halls = GetHalls();
            var invs = GetInvigilators();
            var courses = GetCourses();

            var baseDay = DateTime.Today.AddDays(3); // demo: schedule starts 3 days from now
            var appts = new List<ExamAppointment>();

            // Day 1 – FN
            {
                var (s, e) = Slot(baseDay, ExamSession.FN);
                appts.Add(new ExamAppointment
                {
                    Start = s,
                    End = e,
                    Subject = $"{courses[0].Code} – {courses[0].Title}",
                    Notes = $"Sem {courses[0].Semester} | Dept: {courses[0].Department}",
                    HallId = halls[0].Id,
                    InvigilatorId = invs[0].Id,
                    Background = halls[0].Color
                });
            }

            // Day 1 – AN
            {
                var (s, e) = Slot(baseDay, ExamSession.AN);
                appts.Add(new ExamAppointment
                {
                    Start = s,
                    End = e,
                    Subject = $"{courses[1].Code} – {courses[1].Title}",
                    Notes = $"Sem {courses[1].Semester} | Dept: {courses[1].Department}",
                    HallId = halls[1].Id,
                    InvigilatorId = invs[1].Id,
                    Background = halls[1].Color
                });
            }

            // Day 2 – FN (another course/hall/invigilator)
            {
                var (s, e) = Slot(baseDay.AddDays(1), ExamSession.FN);
                appts.Add(new ExamAppointment
                {
                    Start = s,
                    End = e,
                    Subject = $"{courses[2].Code} – {courses[2].Title}",
                    Notes = $"Sem {courses[2].Semester} | Dept: {courses[2].Department}",
                    HallId = halls[2].Id,
                    InvigilatorId = invs[2].Id,
                    Background = halls[2].Color
                });
            }

            return appts;
        }
    }
}
