using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;
using ExamSchedulerSample.Services;

namespace ExamSchedulerSample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            this.Opacity = 0;
            await this.FadeTo(1, 350, Easing.CubicOut);
        }

        private async void OnOpenSchedulerClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SchedulePage());
        }
    }
}
