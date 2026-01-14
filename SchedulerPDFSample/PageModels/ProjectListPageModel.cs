using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SchedulerPDFSample;
using SchedulerPDFSample;
using SchedulerPDFSample;

namespace SchedulerPDFSample
{
    /// <summary>
    /// ViewModel for the projects list page.
    /// </summary>
    public partial class ProjectListPageModel : ObservableObject
    {
        #region Fields
        private readonly ProjectRepository _projectRepository;
        #endregion

        #region Observable Properties
        [ObservableProperty]
        private List<Project> _projects = [];

        [ObservableProperty]
        private Project? selectedProject;
        #endregion

        #region Constructor
        public ProjectListPageModel(ProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }
        #endregion

        #region Commands
        [RelayCommand]
        private async Task Appearing()
        {
            Projects = await _projectRepository.ListAsync();
        }

        [RelayCommand]
        Task? NavigateToProject(Project project) => project is null ? Task.CompletedTask : Shell.Current.GoToAsync($"project?id={project.ID}");

        [RelayCommand]
        async Task AddProject()
        {
            await Shell.Current.GoToAsync($"project");
        }
        #endregion
    }
}
