namespace SchedulerPDFSample
{
    /// <summary>
    /// Page for creating and editing a single task.
    /// </summary>
    public partial class TaskDetailPage : ContentPage
    {
        /// <summary>
        /// Initializes the page and sets its view model.
        /// </summary>
        /// <param name="model">The task details view model.</param>
        public TaskDetailPage(TaskDetailPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}
