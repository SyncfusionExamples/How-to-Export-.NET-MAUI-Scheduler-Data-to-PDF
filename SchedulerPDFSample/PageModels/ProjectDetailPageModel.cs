using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SchedulerPDFSample
{
    /// <summary>
    /// ViewModel for creating and managing a specific project and its tasks/tags.
    /// </summary>
    /// <summary>
    /// View model for creating and editing a single project, including its tasks and tags.
    /// </summary>
    public partial class ProjectDetailPageModel : ObservableObject, IQueryAttributable, IProjectTaskPageModel
    {
        #region Fields
        private Project? _project;
        private readonly ProjectRepository _projectRepository;
        private readonly TaskRepository _taskRepository;
        private readonly CategoryRepository _categoryRepository;
        private readonly TagRepository _tagRepository;
        private readonly ModalErrorHandler _errorHandler;
        #endregion

        #region Observable Properties
        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private string _description = string.Empty;

        [ObservableProperty]
        private List<ProjectTask> _tasks = [];

        [ObservableProperty]
        private List<Category> _categories = [];

        [ObservableProperty]
        private Category? _category;

        [ObservableProperty]
        private int _categoryIndex = -1;

        [ObservableProperty]
        private List<Tag> _allTags = [];

        public IList<object> SelectedTags { get; set; } = new List<object>();

        [ObservableProperty]
        private IconData _icon;

        [ObservableProperty]
        bool _isBusy;

        [ObservableProperty]
        private List<IconData> _icons = new List<IconData>
        {
            new IconData { Icon = FluentUI.ribbon_24_regular, Description = "Ribbon Icon" },
            new IconData { Icon = FluentUI.ribbon_star_24_regular, Description = "Ribbon Star Icon" },
            new IconData { Icon = FluentUI.trophy_24_regular, Description = "Trophy Icon" },
            new IconData { Icon = FluentUI.badge_24_regular, Description = "Badge Icon" },
            new IconData { Icon = FluentUI.book_24_regular, Description = "Book Icon" },
            new IconData { Icon = FluentUI.people_24_regular, Description = "People Icon" },
            new IconData { Icon = FluentUI.bot_24_regular, Description = "Bot Icon" }
        };
        
        private bool _canDelete;
        
        /// <summary>
        /// Indicates whether the current project can be deleted (i.e., it exists/persisted).
        /// Used by the Delete command's CanExecute binding.
        /// </summary>
        public bool CanDelete
        {
            get => _canDelete;
            set
            {
                if (SetProperty(ref _canDelete, value))
                {
                    DeleteCommand.NotifyCanExecuteChanged();
                }
            }
        }
        #endregion

        #region Computed Properties
        public bool HasCompletedTasks => _project?.Tasks.Any(task => task.IsCompleted) ?? false;
        #endregion

        #region Constructor
        public ProjectDetailPageModel(ProjectRepository projectRepository, TaskRepository taskRepository, CategoryRepository categoryRepository, TagRepository tagRepository, ModalErrorHandler errorHandler)
        {
            _projectRepository = projectRepository;
            _taskRepository = taskRepository;
            _categoryRepository = categoryRepository;
            _tagRepository = tagRepository;
            _errorHandler = errorHandler;
            _icon = _icons.First();
            Tasks = [];
        }
        #endregion

        #region Query Handling
        /// <summary>
        /// Handles Shell navigation query parameters to load or initialize project data.
        /// </summary>
        /// <param name="query">Query parameters passed during navigation.</param>
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("id"))
            {
                int projectId = Convert.ToInt32(query["id"]);
                LoadData(projectId).FireAndForgetSafeAsync(_errorHandler);
            }
            else if (query.ContainsKey("refresh"))
            {
                RefreshData().FireAndForgetSafeAsync(_errorHandler);
            }
            else
            {
                Task.WhenAll(LoadCategories(), LoadTags()).FireAndForgetSafeAsync(_errorHandler);
                _project = new();
                _project.Tags = [];
                _project.Tasks = [];
                Tasks = _project.Tasks;
            }
        }
        #endregion

        #region Data Loading
        private async Task LoadCategories() => Categories = await _categoryRepository.ListAsync();

        private async Task LoadTags() => AllTags = await _tagRepository.ListAsync();

        /// <summary>
        /// Refreshes tasks for the current project from the repository.
        /// </summary>
        private async Task RefreshData()
        {
            if (_project.IsNullOrNew())
            {
                if (_project is not null)
                    Tasks = new(_project.Tasks);

                return;
            }

            Tasks = await _taskRepository.ListAsync(_project.ID);
            _project.Tasks = Tasks;
        }

        /// <summary>
        /// Loads the project by id along with its categories and tags, and initializes fields.
        /// </summary>
        /// <param name="projectId">The identifier of the project to load.</param>
        private async Task LoadData(int projectId)
        {
            try
            {
                IsBusy = true;

                _project = await _projectRepository.GetAsync(projectId);

                if (_project.IsNullOrNew())
                {
                    _errorHandler.HandleError(new Exception($"Project with id {projectId} could not be found."));
                    return;
                }

                Name = _project.Name;
                Description = _project.Description;
                Tasks = _project.Tasks;

                Icon.Icon = _project.Icon;

                Categories = await _categoryRepository.ListAsync();
                Category = Categories?.FirstOrDefault(category => category.ID == _project.CategoryID);
                CategoryIndex = Categories?.FindIndex(category => category.ID == _project.CategoryID) ?? -1;

                var fetchedTags = await _tagRepository.ListAsync();
                foreach (var tag in fetchedTags)
                {
                    tag.IsSelected = _project.Tags.Any(projectTag => projectTag.ID == tag.ID);
                    if (tag.IsSelected)
                    {
                        SelectedTags.Add(tag);
                    }
                }
                AllTags = new(fetchedTags);
            }
            catch (Exception exception)
            {
                _errorHandler.HandleError(exception);
            }
            finally
            {
                IsBusy = false;
                CanDelete = !_project.IsNullOrNew();
                OnPropertyChanged(nameof(HasCompletedTasks));
            }
        }
        #endregion

        #region Commands
        [RelayCommand]
        /// <summary>
        /// Saves a task's completion state change within the project.
        /// </summary>
        /// <param name="task">The task whose completion state changed.</param>
        private async Task TaskCompleted(ProjectTask task)
        {
            await _taskRepository.SaveItemAsync(task);
            OnPropertyChanged(nameof(HasCompletedTasks));
        }

        [RelayCommand]
        /// <summary>
        /// Persists project details, selected tags, and new tasks to the repositories.
        /// </summary>
        private async Task Save()
        {
            if (_project is null)
            {
                _errorHandler.HandleError(new Exception("Project is null. Cannot Save."));
                return;
            }

            _project.Name = Name;
            _project.Description = Description;
            _project.CategoryID = Category?.ID ?? 0;
            _project.Icon = Icon.Icon ?? FluentUI.ribbon_24_regular;
            await _projectRepository.SaveItemAsync(_project);

            foreach (var tag in AllTags)
            {
                if (tag.IsSelected)
                {
                    await _tagRepository.SaveItemAsync(tag, _project.ID);
                }
            }

            foreach (var task in _project.Tasks)
            {
                if (task.ID == 0)
                {
                    task.ProjectID = _project.ID;
                    await _taskRepository.SaveItemAsync(task);
                }
            }

            await Shell.Current.GoToAsync("..");
            await AppShell.DisplayToastAsync("Project saved");
        }

        [RelayCommand]
        /// <summary>
        /// Navigates to the task creation page with current project context.
        /// </summary>
        private async Task AddTask()
        {
            if (_project is null)
            {
                _errorHandler.HandleError(new Exception("Project is null. Cannot navigate to task."));
                return;
            }

            await Shell.Current.GoToAsync($"task",
                new ShellNavigationQueryParameters(){
                    {TaskDetailPageModel.ProjectQueryKey, _project}
                });
        }

        [RelayCommand(CanExecute = nameof(CanDelete))]
        /// <summary>
        /// Deletes the current project when possible and navigates back.
        /// </summary>
        private async Task Delete()
        {
            if (_project.IsNullOrNew())
            {
                await Shell.Current.GoToAsync("..");
                return;
            }

            await _projectRepository.DeleteItemAsync(_project);
            await Shell.Current.GoToAsync("..");
            await AppShell.DisplayToastAsync("Project deleted");
        }

        [RelayCommand]
        /// <summary>
        /// Navigates to the task details page for a task within this project.
        /// </summary>
        /// <param name="task">The task to navigate to.</param>
        private Task NavigateToTask(ProjectTask task) => Shell.Current.GoToAsync($"task?id={task.ID}");

        [RelayCommand]
        /// <summary>
        /// Toggles selection state of a tag and persists the change if the project is saved.
        /// </summary>
        /// <param name="tag">The tag being toggled.</param>
        internal async Task ToggleTag(Tag tag)
        {
            tag.IsSelected = !tag.IsSelected;

            if (!_project.IsNullOrNew())
            {
                if (tag.IsSelected)
                {
                    await _tagRepository.SaveItemAsync(tag, _project.ID);
                }
                else
                {
                    await _tagRepository.DeleteItemAsync(tag, _project.ID);
                }
            }

            AllTags = new(AllTags);
            SemanticScreenReader.Announce($"{tag.Title} {(tag.IsSelected ? "selected" : "unselected")}");
        }

        [RelayCommand]
        /// <summary>
        /// Announces selected icon for accessibility.
        /// </summary>
        /// <param name="icon">Icon data that was selected.</param>
        private void IconSelected(IconData icon)
        {
            SemanticScreenReader.Announce($"{icon.Description} selected");
        }

        [RelayCommand]
        /// <summary>
        /// Deletes completed tasks from this project and refreshes the list.
        /// </summary>
        private async Task CleanTasks()
        {
            var completedTasks = Tasks.Where(task => task.IsCompleted).ToArray();
            foreach (var completedTask in completedTasks)
            {
                await _taskRepository.DeleteItemAsync(completedTask);
                Tasks.Remove(completedTask);
            }

            Tasks = new(Tasks);
            OnPropertyChanged(nameof(HasCompletedTasks));
            await AppShell.DisplayToastAsync("All cleaned up!");
        }

        [RelayCommand]
        /// <summary>
        /// Synchronizes tag selection with the backing collection when the UI selection changes.
        /// </summary>
        /// <param name="parameter">Selection changed event parameter from the view.</param>
        private async Task SelectionChanged(object parameter)
        {
            if (parameter is IEnumerable<object> enumerableParameter)
            {
                var currentSelection = enumerableParameter.OfType<Tag>().ToList();
                var previousSelection = AllTags.Where(tag => tag.IsSelected).ToList();

                // Handle newly selected tags
                foreach (var tag in currentSelection.Except(previousSelection))
                {
                    tag.IsSelected = true;
                    if (!_project.IsNullOrNew())
                    {
                        await _tagRepository.SaveItemAsync(tag, _project.ID);
                    }
                }

                // Handle deselected tags
                foreach (var tag in previousSelection.Except(currentSelection))
                {
                    tag.IsSelected = false;
                    if (!_project.IsNullOrNew())
                    {
                        await _tagRepository.DeleteItemAsync(tag, _project.ID);
                    }
                }
            }
        }
        #endregion
    }
}

