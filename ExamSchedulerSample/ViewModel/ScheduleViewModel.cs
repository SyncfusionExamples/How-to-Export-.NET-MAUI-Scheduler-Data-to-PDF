using System;
using System.Collections.ObjectModel;

namespace ExamSchedulerSample
{
    /// <summary>
    /// Represents the view model responsible for providing scheduler data
    /// (appointments, halls, and invigilators) to the Schedule page.
    /// It also computes the initial <see cref="DisplayDate"/> so that
    /// the scheduler opens aligned with the first exam.
    /// </summary>
    public sealed class ScheduleViewModel
    {
        /// <summary>
        /// Gets the collection of exam appointments displayed in the scheduler.
        /// </summary>
        public ObservableCollection<ExamAppointment> Appointments { get; } = new();

        /// <summary>
        /// Gets the collection of halls used in the scheduler's resource grouping.
        /// </summary>
        public ObservableCollection<Hall> Halls { get; } = new();

        /// <summary>
        /// Gets the collection of invigilators available in the appointment assignments.
        /// </summary>
        public ObservableCollection<Invigilator> Invigilators { get; } = new();

        /// <summary>
        /// Gets the date that the scheduler should first display.
        /// Defaults to the start date of the earliest appointment.
        /// If no appointments exist, defaults to today's date.
        /// </summary>
        public DateTime DisplayDate { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleViewModel"/> class,
        /// loading sample halls, invigilators, and appointments from the
        /// <see cref="SampleDataService"/>. The <see cref="DisplayDate"/> is set
        /// automatically based on the earliest appointment.
        /// </summary>
        public ScheduleViewModel()
        {
            foreach (var hall in SampleDataService.GetHalls())
                Halls.Add(hall);

            foreach (var invigilator in SampleDataService.GetInvigilators())
                Invigilators.Add(invigilator);

            foreach (var appointment in SampleDataService.GetExamAppointments())
                Appointments.Add(appointment);

            DisplayDate =
                Appointments.Count > 0
                ? Appointments[0].Start.Date
                : DateTime.Today;
        }
    }
}
