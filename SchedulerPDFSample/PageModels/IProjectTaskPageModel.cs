using CommunityToolkit.Mvvm.Input;

namespace SchedulerPDFSample
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}