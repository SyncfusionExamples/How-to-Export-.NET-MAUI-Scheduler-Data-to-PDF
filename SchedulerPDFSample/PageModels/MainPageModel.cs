
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace SchedulerPDFSample
{
    /// <summary>
    /// ViewModel for the dashboard page. Loads/summarizes projects and tasks.
    /// </summary>
    public partial class MainPageModel : ObservableObject, IProjectTaskPageModel
    {
        #region Fields
        private bool _isNavigatedTo;
        private bool _dataLoaded;
        private readonly ProjectRepository _projectRepository;
        private readonly TaskRepository _taskRepository;
        private readonly CategoryRepository _categoryRepository;
        private readonly ModalErrorHandler _errorHandler;
        private readonly SeedDataService _seedDataService;
        #endregion

        #region Observable Properties
        [ObservableProperty]
        private List<CategoryChartData> _todoCategoryData = [];

        [ObservableProperty]
        private List<Brush> _todoCategoryColors = [];

        [ObservableProperty]
        private List<ProjectTask> _tasks = [];

        [ObservableProperty]
        private ObservableCollection<SchedulerAppointment> _dashboardAppointments = new();

        [ObservableProperty]
        private List<Project> _projects = [];

        [ObservableProperty]
        bool _isBusy;

        [ObservableProperty]
        bool _isRefreshing;

        [ObservableProperty]
        private string _today = DateTime.Now.ToString("dddd, MMM d");

        [ObservableProperty]
        private Project? selectedProject;
        #endregion

        #region Computed Properties
        public bool HasCompletedTasks => Tasks?.Any(task => task.IsCompleted) ?? false;
        #endregion

        #region Constructor
        public MainPageModel(SeedDataService seedDataService, ProjectRepository projectRepository,
            TaskRepository taskRepository, CategoryRepository categoryRepository, ModalErrorHandler errorHandler)
        {
            _projectRepository = projectRepository;
            _taskRepository = taskRepository;
            _categoryRepository = categoryRepository;
            _errorHandler = errorHandler;
            _seedDataService = seedDataService;
        }
        #endregion

        #region Data Loading
        /// <summary>
        /// Loads projects, tasks, and categories, computes chart data and dashboard appointments,
        /// normalizes categories, and refreshes observable properties for the main dashboard.
        /// </summary>
        /// <returns>A task that completes when data loading and processing is finished.</returns>
        private async Task LoadData()
        {
            try
            {
                IsBusy = true;

                Projects = await _projectRepository.ListAsync();

                // Load tasks and categories
                Tasks = await _taskRepository.ListAsync();
                var categories = await _categoryRepository.ListAsync();

                // normalize helper
                static string Norm(string? s) => (s ?? string.Empty).Trim();

                // Consistent palette (ignore case) built from DB categories
                var palette = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var category in categories)
                    if (!string.IsNullOrWhiteSpace(category.Title))
                        palette[Norm(category.Title)] = category.Color;

                const string Uncategorized = "Uncategorized";
                if (!palette.ContainsKey(Uncategorized))
                    palette[Uncategorized] = "#808080"; // neutral gray

                // Group tasks by their Category label (normalize); unknown categories allowed
                // Map task.Category to a known Category title. If task.Category holds a project name (from earlier versions),
                // translate it to that project's Category title. Fallback to Uncategorized.
                string MapToCategory(ProjectTask task)
                {
                    var label = Norm(task.Category);
                    if (!string.IsNullOrWhiteSpace(label) && palette.ContainsKey(label))
                        return label;

                    // try by ProjectID
                    var proj = Projects.FirstOrDefault(project => project.ID == task.ProjectID);
                    var projCat = proj?.Category?.Title;
                    if (!string.IsNullOrWhiteSpace(projCat)) return Norm(projCat);
                    if (proj is not null && proj.CategoryID > 0)
                    {
                        var catById = categories.FirstOrDefault(category => category.ID == proj.CategoryID)?.Title;
                        if (!string.IsNullOrWhiteSpace(catById)) return Norm(catById);
                    }

                    // try match label to a project name
                    if (!string.IsNullOrWhiteSpace(label))
                    {
                        var projectByName = Projects.FirstOrDefault(project => string.Equals(Norm(project.Name), label, StringComparison.OrdinalIgnoreCase));
                        var catTitle = projectByName?.Category?.Title;
                        if (!string.IsNullOrWhiteSpace(catTitle)) return Norm(catTitle);
                    }

                    return string.IsNullOrWhiteSpace(label) ? Uncategorized : label; // keep original if not found
                }

                var taskCounts = Tasks
                    .GroupBy(task => MapToCategory(task), StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(group => group.Key, group => group.Count(), StringComparer.OrdinalIgnoreCase);

                // Auto-migrate tasks with missing/unknown category: set to mapped category and persist
                bool allUncategorized = taskCounts.Count == 1 && taskCounts.Keys.FirstOrDefault() == Uncategorized;
                if (allUncategorized)
                {
                    foreach (var task in Tasks)
                    {
                        var mapped = MapToCategory(task);
                        if (!string.Equals(Norm(task.Category), mapped, StringComparison.OrdinalIgnoreCase))
                        {
                            task.Category = mapped;
                            _taskRepository.SaveItemAsync(task).FireAndForgetSafeAsync(_errorHandler);
                        }
                    }

                    // Recompute counts after migration
                    taskCounts = Tasks
                        .GroupBy(task => MapToCategory(task), StringComparer.OrdinalIgnoreCase)
                        .ToDictionary(group => group.Key, group => group.Count(), StringComparer.OrdinalIgnoreCase);
                }

                // Determine if any non-empty categories exist
                bool anyTaskCategories = taskCounts.Sum(kv => kv.Value) > 0;

                var categoryChartData = new List<CategoryChartData>();
                var categoryChartColors = new List<Brush>();

                // Show chart slices per Project; colors come from the project's assigned Category
                foreach (var project in Projects.Where(project => project is not null))
                {
                    var count = project!.Tasks?.Count ?? 0;
                    if (count <= 0) continue; // only show projects that have tasks

                    var title = string.IsNullOrWhiteSpace(project.Name) ? "Project" : project.Name;
                    categoryChartData.Add(new(title, count));

                    // Use the project's category color (by CategoryID) if available; otherwise a neutral color
                    var brush = categories.FirstOrDefault(c => c.ID == project.CategoryID)?.ColorBrush
                        ?? project.Category?.ColorBrush
                        ?? new SolidColorBrush(Microsoft.Maui.Graphics.Colors.SlateGray);
                    categoryChartColors.Add(brush);
                }

                TodoCategoryData = categoryChartData;
                TodoCategoryColors = categoryChartColors;

                // sort tasks by start time then title for consistent dashboard order
                Tasks = Tasks.OrderBy(task => task.StartTime == default ? DateTime.MaxValue : task.StartTime)
                             .ThenBy(task => task.Title)
                             .ToList();

                // Build scheduler appointments for dashboard using the same palette and category mapping
                DashboardAppointments.Clear();

                var weekStart = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek); // Sunday
                foreach (var task in Tasks.Where(task => !task.IsCompleted))
                {
                    var categoryLabel = MapToCategory(task);

                    var start = task.StartTime == default
                        ? weekStart.AddDays(((int)(task.ID % 7 + 7)) % 7).AddHours(9 + ((int)(task.ID % 4 + 4)) % 4 * 2)
                        : task.StartTime;
                    var end = task.EndTime == default || task.EndTime <= task.StartTime ? start.AddHours(1) : task.EndTime;

                    var colorHex = palette.TryGetValue(categoryLabel, out var hex) ? hex : palette[Uncategorized];

                    DashboardAppointments.Add(new SchedulerAppointment
                    {
                        Subject = string.IsNullOrWhiteSpace(task.Title) ? "Task" : task.Title,
                        StartTime = start,
                        EndTime = end,
                        Background = new SolidColorBrush(Microsoft.Maui.Graphics.Color.FromArgb(colorHex)),
                        Notes = categoryLabel
                    });
                }
            }
            finally
            {
                IsBusy = false;
                OnPropertyChanged(nameof(HasCompletedTasks));
            }
        }

        /// <summary>
        /// Ensures initial seed data exists in the local store. Seeds on first run or when the database is empty.
        /// </summary>
        /// <param name="seedDataService">Service responsible for loading seed data.</param>
        /// <returns>A task that completes after seeding verification.</returns>
        private async Task InitData(SeedDataService seedDataService)
        {
            bool isSeeded = Preferences.Default.ContainsKey("is_seeded");

            if (!isSeeded)
            {
                await seedDataService.LoadSeedDataAsync();
                Preferences.Default.Set("is_seeded", true);
                return;
            }

            // If DB was recreated or is empty, reseed automatically
            var existingProjects = await _projectRepository.ListAsync();
            var existingCategories = await _categoryRepository.ListAsync();
            var existingTasks = await _taskRepository.ListAsync();
            if ((existingProjects?.Count ?? 0) == 0 && (existingCategories?.Count ?? 0) == 0 && (existingTasks?.Count ?? 0) == 0)
            {
                await seedDataService.LoadSeedDataAsync();
            }
        }
        #endregion

        #region Commands
        [RelayCommand]
        /// <summary>
        /// Navigates to the full scheduler page.
        /// </summary>
        /// <returns>A navigation task.</returns>
        private Task OpenScheduler() => Shell.Current.GoToAsync("//shift");

        [RelayCommand]
        /// <summary>
        /// Pull-to-refresh command that reloads dashboard data with error handling.
        /// </summary>
        /// <returns>A task representing the refresh operation.</returns>
        private async Task Refresh()
        {
            try
            {
                IsRefreshing = true;
                await LoadData();
            }
            catch (Exception exception)
            {
                _errorHandler.HandleError(exception);
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        /// <summary>
        /// Marks that the page has been navigated to, used for refresh logic.
        /// </summary>
        private void NavigatedTo() => _isNavigatedTo = true;

        [RelayCommand]
        /// <summary>
        /// Marks that the page has been navigated away from, used for refresh logic.
        /// </summary>
        private void NavigatedFrom() => _isNavigatedTo = false;

        [RelayCommand]
        /// <summary>
        /// Lifecycle hook for page appearing; triggers initial seeding and subsequent data refreshes.
        /// </summary>
        /// <returns>A task representing the appearing workflow.</returns>
        private async Task Appearing()
        {
            if (!_dataLoaded)
            {
                await InitData(_seedDataService);
                _dataLoaded = true;
                await Refresh();
            }
            else if (!_isNavigatedTo)
            {
                await Refresh();
            }
        }

        [RelayCommand]
        /// <summary>
        /// Persists a task's completion state change.
        /// </summary>
        /// <param name="task">The task whose completion state changed.</param>
        /// <returns>A task for the save operation.</returns>
        private Task TaskCompleted(ProjectTask task)
        {
            OnPropertyChanged(nameof(HasCompletedTasks));
            return _taskRepository.SaveItemAsync(task);
        }

        [RelayCommand]
        /// <summary>
        /// Navigates to the task creation page.
        /// </summary>
        /// <returns>A navigation task.</returns>
        private Task AddTask() => Shell.Current.GoToAsync($"task");

        [RelayCommand]
        /// <summary>
        /// Navigates to the project details page.
        /// </summary>
        /// <param name="project">The project to navigate to.</param>
        /// <returns>A navigation task, or null if project is null.</returns>
        private Task? NavigateToProject(Project project) => project is null ? null : Shell.Current.GoToAsync($"project?id={project.ID}");

        [RelayCommand]
        /// <summary>
        /// Navigates to the task details page.
        /// </summary>
        /// <param name="task">The task to navigate to.</param>
        /// <returns>A navigation task.</returns>
        private Task NavigateToTask(ProjectTask task) => Shell.Current.GoToAsync($"task?id={task.ID}");

        [RelayCommand]
        /// <summary>
        /// Deletes all completed tasks and refreshes the dashboard list.
        /// </summary>
        /// <returns>A task that completes when cleanup is finished.</returns>
        private async Task CleanTasks()
        {
            var completedTasks = Tasks.Where(task => task.IsCompleted).ToList();
            foreach (var completedTask in completedTasks)
            {
                await _taskRepository.DeleteItemAsync(completedTask);
                Tasks.Remove(completedTask);
            }

            OnPropertyChanged(nameof(HasCompletedTasks));
            Tasks = new(Tasks);
            await AppShell.DisplayToastAsync("All cleaned up!");
        }
        #endregion
    }
}
