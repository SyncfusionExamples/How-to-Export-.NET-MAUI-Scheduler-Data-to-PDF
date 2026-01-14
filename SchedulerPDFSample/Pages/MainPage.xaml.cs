namespace SchedulerPDFSample
{
    /// <summary>
    /// Dashboard page that hosts the main task and project overview.
    /// </summary>
    public partial class MainPage : ContentPage
    {
        /// <summary>
        /// Creates the dashboard page and sets its binding context.
        /// </summary>
        /// <param name="model">View model for the main page.</param>
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}
