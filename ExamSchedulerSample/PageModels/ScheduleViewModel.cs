
using System.Collections.ObjectModel;
using Microsoft.Maui.Graphics;

namespace ExamSchedulerSample
{
    public sealed class ScheduleViewModel
    {
        public ObservableCollection<ExamAppointment> Appointments { get; } = new();
        public ObservableCollection<Hall> Halls { get; } = new();
        public ObservableCollection<Invigilator> Invigilators { get; } = new();

        public ScheduleViewModel()
        {
            foreach (var h in SampleDataService.GetHalls()) Halls.Add(h);
            foreach (var t in SampleDataService.GetInvigilators()) Invigilators.Add(t);
            foreach (var a in SampleDataService.GetExamAppointments()) Appointments.Add(a);
        }
    }
}
