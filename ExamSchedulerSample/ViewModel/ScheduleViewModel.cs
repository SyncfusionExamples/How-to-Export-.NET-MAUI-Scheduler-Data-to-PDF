using System.Collections.ObjectModel;

namespace ExamSchedulerSample
{
    /// <summary>
    /// Represents the view model that provides scheduler data to the Schedule page. The data set is filtered by the selected <see cref="Program"/>.
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
        /// </summary>
        public DateTime DisplayDate { get; private set; }

        /// <summary>
        /// Gets the currently-selected academic program (UG, PG, MBA).
        /// </summary>
        public ProgramType Program { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleViewModel"/> class with the default program.
        /// </summary>
        public ScheduleViewModel() : this(ProgramType.UG)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleViewModel"/> class and loads halls, invigilators, and program-filtered appointments.
        /// </summary>
        public ScheduleViewModel(ProgramType program)
        {
            Program = program;
            LoadStaticData();
            LoadProgramData(program);
        }

        /// <summary>
        /// Reloads the scheduler data for the specified <paramref name="program"/>, replacing the appointment set and updating <see cref="DisplayDate"/>.
        /// </summary>
        public void LoadData(ProgramType program)
        {
            Program = program;
            LoadProgramData(program);
        }

        /// <summary>
        /// Loads static resources (Halls, Invigilators). Called once from the constructor.
        /// </summary>
        private void LoadStaticData()
        {
            Halls.Clear();
            foreach (var hall in SampleDataPage.GetHalls())
            {
                Halls.Add(hall);
            }

            Invigilators.Clear();
            foreach (var invigilator in SampleDataPage.GetInvigilators())
            {
                Invigilators.Add(invigilator);
            }
        }

        /// <summary>
        /// Loads appointments for the selected program and computes the display date.
        /// </summary>
        private void LoadProgramData(ProgramType program)
        {
            Appointments.Clear();
            foreach (var appointment in SampleDataPage.GetExamAppointments(program))
            {
                Appointments.Add(appointment);
            }

            DisplayDate = Appointments.Count > 0 ? Appointments[0].Start.Date : DateTime.Today;
        }
    }
}