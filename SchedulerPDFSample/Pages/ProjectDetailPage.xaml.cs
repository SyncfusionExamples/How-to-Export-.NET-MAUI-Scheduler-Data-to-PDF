namespace SchedulerPDFSample
{
    /// <summary>
    /// Page for creating and editing a project's details, tasks, and tags.
    /// </summary>
    public partial class ProjectDetailPage : ContentPage
    {
        /// <summary>
        /// Initializes the page and assigns its view model.
        /// </summary>
        /// <param name="model">The project details view model.</param>
        public ProjectDetailPage(ProjectDetailPageModel model)
        {
            InitializeComponent();

            BindingContext = model;
        }
    }
}
