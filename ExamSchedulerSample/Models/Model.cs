using System;
using System.Collections.Generic;
using Microsoft.Maui.Graphics;

namespace ExamSchedulerSample
{
    public enum ExamSession { FN, AN } // Forenoon/Afternoon

    public sealed class Course
    {
        public string Code { get; set; } = "";
        public string Title { get; set; } = "";
        public int Semester { get; set; }
        public string Department { get; set; } = "";
    }

    public sealed class Hall
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public int Capacity { get; set; }
        public Color Color { get; set; } = Colors.SlateBlue;
    }

    public sealed class Invigilator
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Department { get; set; } = "";
        public Color Color { get; set; } = Colors.Teal;
    }

    // The business appointment object the Scheduler will bind to
    public sealed class ExamAppointment
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Subject { get; set; } = "";         // e.g., "CS101 – Data Structures"
        public string Notes { get; set; } = "";           // e.g., "Semester 3 | Dept: CSE"
        public string HallId { get; set; } = "";          // for resource view mapping
        public string InvigilatorId { get; set; } = "";   // for resource view mapping (optional)
        public bool IsAllDay => false;
        public Color Background { get; set; } = Colors.CornflowerBlue;
    }

    // Simple records for PDF tables
    public sealed class HallAvailabilityRow
    {
        public DateTime Date { get; set; }
        public ExamSession Session { get; set; }
        public string Hall { get; set; } = "";
        public int Capacity { get; set; }
        public string Status { get; set; } = ""; // "Available" / "Allocated"
    }

    public sealed class InvigilatorDutyRow
    {
        public DateTime Date { get; set; }
        public ExamSession Session { get; set; }
        public string Invigilator { get; set; } = "";
        public string Hall { get; set; } = "";
        public string Course { get; set; } = "";
    }
}
