using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SchedulerPDFSample
{
    /// <summary>
    /// ViewModel to manage metadata (Categories and Tags).
    /// </summary>
    /// <summary>
    /// View model that manages Categories, Projects, and Tags administration.
    /// </summary>
    public partial class ManageMetaPageModel : ObservableObject
    {
        #region Fields
        private readonly CategoryRepository _categoryRepository;
        private readonly TagRepository _tagRepository;
        private readonly ProjectRepository _projectRepository;
        private readonly SeedDataService _seedDataService;
        #endregion

        #region Observable Properties
        [ObservableProperty]
        private ObservableCollection<Category> _categories = [];

        [ObservableProperty]
        private ObservableCollection<Project> _projects = [];

        [ObservableProperty]
        private ObservableCollection<Tag> _tags = [];
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new instance of the metadata management view model.
        /// </summary>
        /// <param name="categoryRepository">Repository for categories.</param>
        /// <param name="tagRepository">Repository for tags.</param>
        /// <param name="projectRepository">Repository for projects.</param>
        /// <param name="seedDataService">Service that seeds sample data.</param>
        public ManageMetaPageModel(CategoryRepository categoryRepository, TagRepository tagRepository, ProjectRepository projectRepository, SeedDataService seedDataService)
        {
            _categoryRepository = categoryRepository;
            _tagRepository = tagRepository;
            _projectRepository = projectRepository;
            _seedDataService = seedDataService;
        }
        #endregion

        #region Data Loading
        /// <summary>
        /// Loads categories, projects, and tags from repositories.
        /// </summary>
        private async Task LoadData()
        {
            var allCategories = await _categoryRepository.ListAsync();
            Categories = new ObservableCollection<Category>(allCategories);

            var allProjects = await _projectRepository.ListAsync();
            // Populate Project.Category for picker selection
            foreach (var project in allProjects)
                project.Category = Categories.FirstOrDefault(category => category.ID == project.CategoryID);
            Projects = new ObservableCollection<Project>(allProjects);

            var allTags = await _tagRepository.ListAsync();
            Tags = new ObservableCollection<Tag>(allTags);
        }
        #endregion

        #region Commands
        [RelayCommand]
        /// <summary>
        /// Invoked when the page appears; loads metadata.
        /// </summary>
        private Task Appearing() => LoadData();

        [RelayCommand]
        /// <summary>
        /// Saves all category changes to the repository.
        /// </summary>
        private async Task SaveCategories()
        {
            foreach (var category in Categories)
            {
                await _categoryRepository.SaveItemAsync(category);
            }

            await AppShell.DisplayToastAsync("Categories saved");
            SemanticScreenReader.Announce("Categories saved");
        }

        [RelayCommand]
        /// <summary>
        /// Deletes a category from the repository.
        /// </summary>
        /// <param name="category">The category to delete.</param>
        private async Task DeleteCategory(Category category)
        {
            Categories.Remove(category);
            await _categoryRepository.DeleteItemAsync(category);
            await AppShell.DisplayToastAsync("Category deleted");
            SemanticScreenReader.Announce("Category deleted");
        }

        [RelayCommand]
        /// <summary>
        /// Adds a new category and persists it.
        /// </summary>
        private async Task AddCategory()
        {
            var category = new Category();
            Categories.Add(category);
            await _categoryRepository.SaveItemAsync(category);
            await AppShell.DisplayToastAsync("Category added");
            SemanticScreenReader.Announce("Category added");
        }

        [RelayCommand]
        /// <summary>
        /// Saves all project changes to the repository.
        /// </summary>
        private async Task SaveProjects()
        {
            foreach (var project in Projects)
            {
                project.CategoryID = project.Category?.ID ?? 0;
                await _projectRepository.SaveItemAsync(project);
            }
            await AppShell.DisplayToastAsync("Projects saved");
            SemanticScreenReader.Announce("Projects saved");
        }

        [RelayCommand]
        /// <summary>
        /// Deletes a project from the repository.
        /// </summary>
        /// <param name="project">The project to delete.</param>
        private async Task DeleteProject(Project project)
        {
            Projects.Remove(project);
            await _projectRepository.DeleteItemAsync(project);
            await AppShell.DisplayToastAsync("Project deleted");
            SemanticScreenReader.Announce("Project deleted");
        }

        [RelayCommand]
        /// <summary>
        /// Adds a new project with defaults and persists it.
        /// </summary>
        private async Task AddProject()
        {
            var project = new Project { Name = "New Project", Description = string.Empty, Icon = Fonts.FluentUI.ribbon_24_regular };
            Projects.Add(project);
            await _projectRepository.SaveItemAsync(project);
            await AppShell.DisplayToastAsync("Project added");
            SemanticScreenReader.Announce("Project added");
        }

        [RelayCommand]
        /// <summary>
        /// Saves all tag changes to the repository.
        /// </summary>
        private async Task SaveTags()
        {
            foreach (var tag in Tags)
            {
                await _tagRepository.SaveItemAsync(tag);
            }

            await AppShell.DisplayToastAsync("Tags saved");
            SemanticScreenReader.Announce("Tags saved");
        }

        [RelayCommand]
        /// <summary>
        /// Deletes a tag from the repository.
        /// </summary>
        /// <param name="tag">The tag to delete.</param>
        private async Task DeleteTag(Tag tag)
        {
            Tags.Remove(tag);
            await _tagRepository.DeleteItemAsync(tag);
            await AppShell.DisplayToastAsync("Tag deleted");
            SemanticScreenReader.Announce("Tags deleted");
        }

        [RelayCommand]
        /// <summary>
        /// Adds a new tag and persists it.
        /// </summary>
        private async Task AddTag()
        {
            var tag = new Tag();
            Tags.Add(tag);
            await _tagRepository.SaveItemAsync(tag);
            await AppShell.DisplayToastAsync("Tag added");
            SemanticScreenReader.Announce("Tags added");
        }

        [RelayCommand]
        /// <summary>
        /// Resets seeded data and navigates back to the dashboard.
        /// </summary>
        private async Task Reset()
        {
            Preferences.Default.Remove("is_seeded");
            await _seedDataService.LoadSeedDataAsync();
            Preferences.Default.Set("is_seeded", true);
            await Shell.Current.GoToAsync("//main");
        }
        #endregion
    }
}

