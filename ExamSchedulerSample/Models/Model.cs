using Microsoft.Maui.Graphics;
using System;
using System.Collections.ObjectModel;
using System.Xml;

namespace ExamSchedulerSample
{
    /// <summary>
    /// Represents the exam session times:
    /// <c>FN</c> → Forenoon (Morning),
    /// <c>AN</c> → Afternoon.
    /// </summary>
    public enum ExamSession { FN, AN }

    /// <summary>
    /// Represents an academic course containing basic information
    /// such as code, title, semester, and department.
    /// </summary>
    public sealed class Course
    {
        /// <summary>
        /// Gets or sets the course code (e.g., CS101).
        /// </summary>
        public string Code { get; set; } = "";

        /// <summary>
        /// Gets or sets the full course title.
        /// </summary>
        public string Title { get; set; } = "";

        /// <summary>
        /// Gets or sets the semester number in which the course is offered.
        /// </summary>
        public int Semester { get; set; }

        /// <summary>
        /// Gets or sets the offering department of the course.
        /// </summary>
        public string Department { get; set; } = "";
    }

    /// <summary>
    /// Represents an exam hall with name, capacity, and display color.
    /// </summary>
    public sealed class Hall
    {
        /// <summary>
        /// Gets or sets the unique hall identifier.
        /// </summary>
        public string Id { get; set; } = "";

        /// <summary>
        /// Gets or sets the hall name (e.g., "Main Hall").
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Gets or sets the seating capacity of the hall.
        /// </summary>
        public int Capacity { get; set; }

        /// <summary>
        /// Gets or sets the display color used in scheduler UI.
        /// </summary>
        public Color Color { get; set; } = Colors.SlateBlue;

        /// <summary>
        /// Gets a brush version of <see cref="Color"/> for use in UI bindings.
        /// </summary>
        public Brush BackgroundBrush => new SolidColorBrush(Color);
    }

    /// <summary>
    /// Represents an invigilator with identifying and display information.
    /// </summary>
    public sealed class Invigilator
    {
        /// <summary>
        ///  Gets or sets the unique invigilator identifier.
        /// </summary>
        public string Id { get; set; } = "";

        /// <summary>
        /// Gets or sets the invigilator's full name.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Gets or sets the department to which the invigilator belongs.
        /// </summary>
        public string Department { get; set; } = "";

        /// <summary>
        /// Gets or sets the display color for this invigilator in the scheduler.
        /// </summary>
        public Color Color { get; set; } = Colors.Teal;

    }

    /// <summary>
    /// Represents a schedulable exam appointment entry used by the Scheduler view.
    /// Supports resource grouping by hall and invigilator.
    /// </summary>
    public sealed class ExamAppointment
    {

        /// <summary>
        /// Gets or sets the exam start date/time.
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// Gets or sets the exam end date/time.
        /// </summary>
        public DateTime End { get; set; }

        /// <summary>
        /// Gets or sets the primary subject (course) name displayed on the scheduler bar.
        /// </summary>
        public string Subject { get; set; } = "";

        /// <summary>
        /// Gets or sets additional information displayed inside the appointment.
        /// </summary>
        public string Notes { get; set; } = "";

        /// <summary>
        /// Gets or sets the hall identifier linked to the appointment resource.
        /// </summary>
        public string HallId { get; set; } = "";

        /// <summary>
        /// Gets or sets the hall name for display in PDF or UI grids.
        /// </summary>
        public string HallName { get; set; } = "";

        /// <summary>
        /// Gets or sets the invigilator identifier linked to the appointment resource.
        /// </summary>
        public string InvigilatorId { get; set; } = "";

        /// <summary>
        /// Gets or sets the invigilator name used for display.
        /// </summary>
        public string InvigilatorName { get; set; } = "";

        /// <summary>
        /// Gets or sets the background color for the appointment in the scheduler.
        /// </summary>
        public Color Background { get; set; } = Colors.CornflowerBlue;

        /// <summary>
        /// Gets a brush version of <see cref="Background"/> for UI visuals.
        /// </summary>
        public Brush BackgroundBrush => new SolidColorBrush(Background);

        /// <summary>
        /// Gets the resource identifier collection used by Scheduler for grouping.
        /// Typically contains HallId or InvigilatorId depending on setup.
        /// </summary>
        public ObservableCollection<object> ResourceIds { get; } = new();
    }

    /// <summary>
    /// Represents data used for generating hall availability rows in the PDF export.
    /// </summary>
    public sealed class HallAvailabilityRow
    {
        /// <summary>
        /// Gets or sets the exam date.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the exam session (FN or AN).
        /// </summary>
        public ExamSession Session { get; set; }

        /// <summary>
        /// Gets or sets the hall name.
        /// </summary>
        public string Hall { get; set; } = "";

        /// <summary>
        /// Gets or sets the hall seating capacity.
        /// </summary>
        public int Capacity { get; set; }

        /// <summary>
        /// Gets or sets the status of the hall
        /// </summary>
        public string Status { get; set; } = "";
    }

    /// <summary>
    /// Represents a row for invigilator duty assignment used during PDF export.
    /// </summary>
    public sealed class InvigilatorDutyRow
    {
        /// <summary>
        /// Gets or sets the exam date.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the exam session (FN or AN).
        /// </summary>
        public ExamSession Session { get; set; }

        /// <summary>
        /// Gets or sets the invigilator name.
        /// </summary>
        public string Invigilator { get; set; } = "";

        /// <summary>
        /// Gets or sets the hall name where the invigilator is assigned.
        /// </summary>
        public string Hall { get; set; } = "";

        /// <summary>
        /// Gets or sets the exam course/subject the duty is linked to.
        /// </summary>
        public string Course { get; set; } = "";
    }
}
