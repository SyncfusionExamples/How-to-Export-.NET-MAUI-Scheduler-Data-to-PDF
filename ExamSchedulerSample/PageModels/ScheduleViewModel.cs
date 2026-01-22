
// File: ScheduleViewModel.cs
using System;
using System.Collections.ObjectModel;

namespace ExamSchedulerSample
{
    /// <summary>
    /// Exposes scheduler data to the page and sets a DisplayDate
    /// aligned to the first appointment so items are visible immediately.
    /// </summary>
    public sealed class ScheduleViewModel
    {
        public ObservableCollection<ExamAppointment> Appointments { get; } = new();
        public ObservableCollection<Hall> Halls { get; } = new();
        public ObservableCollection<Invigilator> Invigilators { get; } = new();

        public DateTime DisplayDate { get; }

        public ScheduleViewModel()
        {
            foreach (var h in SampleDataService.GetHalls())
                Halls.Add(h);

            foreach (var t in SampleDataService.GetInvigilators())
                Invigilators.Add(t);

            foreach (var a in SampleDataService.GetExamAppointments())
                Appointments.Add(a);

            DisplayDate = Appointments.Count > 0 ? Appointments[0].Start.Date : DateTime.Today;
        }
    }
}
