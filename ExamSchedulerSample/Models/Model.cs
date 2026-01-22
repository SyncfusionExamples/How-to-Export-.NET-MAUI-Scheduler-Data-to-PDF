
// File: Models.cs
using System;
using System.Collections.ObjectModel;
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

        // Brush for Scheduler resource header background mapping (if needed)
        public Brush BackgroundBrush => new SolidColorBrush(Color);
    }

    public sealed class Invigilator
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Department { get; set; } = "";
        public Color Color { get; set; } = Colors.Teal;

        public Brush BackgroundBrush => new SolidColorBrush(Color);
    }

    /// <summary>
    /// Appointment the Scheduler binds to (supports ResourceView).
    /// </summary>
    public sealed class ExamAppointment
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        // What you'll see on the bar
        public string Subject { get; set; } = "";      // e.g., "CS101 – Data Structures"
        public string Notes { get; set; } = "";        // e.g., "Semester 3 | Dept: CSE"

        // Resource mapping
        public string HallId { get; set; } = "";
        public string HallName { get; set; } = "";     // used in UI template
        public string InvigilatorId { get; set; } = "";
        public string InvigilatorName { get; set; } = "";

        public bool IsAllDay => false;

        // Colors & brushes
        public Color Background { get; set; } = Colors.CornflowerBlue;
        public Brush BackgroundBrush => new SolidColorBrush(Background);

        // REQUIRED for ResourceView (we're grouping by Halls)
        public ObservableCollection<object> ResourceIds { get; } = new();
    }

    // Simple rows for PDF export (unchanged)
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
