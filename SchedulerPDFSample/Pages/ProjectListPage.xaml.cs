namespace SchedulerPDFSample
{
    /// <summary>
    /// Page that lists projects and allows navigation to details.
    /// </summary>
    public partial class ProjectListPage : ContentPage
    {
        /// <summary>
        /// Initializes the project list page and assigns its view model.
        /// </summary>
        /// <param name="model">The project list view model.</param>
        public ProjectListPage(ProjectListPageModel model)
        {
            BindingContext = model;
            InitializeComponent();
        }
    }
}
