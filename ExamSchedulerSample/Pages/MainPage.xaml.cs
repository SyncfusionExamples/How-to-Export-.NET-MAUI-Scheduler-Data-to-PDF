namespace ExamSchedulerSample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OpenUg_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SchedulePage(ProgramType.UG));
        }

        private async void OpenPg_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SchedulePage(ProgramType.PG));
        }

        private async void OpenMba_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SchedulePage(ProgramType.MBA));
        }
    }
}