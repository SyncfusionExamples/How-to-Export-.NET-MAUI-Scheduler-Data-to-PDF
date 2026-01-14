namespace SchedulerPDFSample
{
    /// <summary>
    /// Administration page for managing categories, projects, and tags.
    /// </summary>
    public partial class ManageMetaPage : ContentPage
    {
        /// <summary>
        /// Initializes the administration page and sets its view model.
        /// </summary>
        /// <param name="model">The metadata management view model.</param>
        public ManageMetaPage(ManageMetaPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}
