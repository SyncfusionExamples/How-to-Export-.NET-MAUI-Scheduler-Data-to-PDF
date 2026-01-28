namespace ExamSchedulerSample
{
    /// <summary>
    /// Represents the introductory landing page of the application.
    /// Allows the user to select their academic program (UG, PG, MBA) and navigate to the corresponding exam schedule page.
    /// </summary>
    public partial class MainPage : ContentPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class and loads the XAML user interface.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the click event of the UG button and navigates to the UG exam schedule page.
        /// </summary>
        /// <param name="sender">The source of the event (UG button).</param>
        /// <param name="e">Event arguments.</param>
        private async void OpenUg_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SchedulePage(ProgramType.UG));
        }

        /// <summary>
        /// Handles the click event of the PG button and navigates to the PG exam schedule page.
        /// </summary>
        /// <param name="sender">The source of the event (PG button).</param>
        /// <param name="e">Event arguments.</param>
        private async void OpenPg_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SchedulePage(ProgramType.PG));
        }

        /// <summary>
        /// Handles the click event of the MBA button and navigates to the MBA exam schedule page.
        /// </summary>
        /// <param name="sender">The source of the event (MBA button).</param>
        /// <param name="e">Event arguments.</param>
        private async void OpenMba_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SchedulePage(ProgramType.MBA));
        }
    }
}