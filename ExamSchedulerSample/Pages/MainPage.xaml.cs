using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;
using ExamSchedulerSample.Services;

namespace ExamSchedulerSample
{
    /// <summary>
    /// The application's introductory landing page containing
    /// a simple fade‑in animation and a navigation button that
    /// leads the user into the main scheduling workflow.
    /// </summary>
    public partial class MainPage : ContentPage
    {
        /// <summary>
        /// Initializes the MainPage component and loads the UI defined in XAML.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles page lifecycle appearance.  
        /// Applies a fade‑in animation when the page becomes visible.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Start hidden, then fade in smoothly.
            this.Opacity = 0;
            await this.FadeTo(1, 350, Easing.CubicOut);
        }

        /// <summary>
        /// Handles the Enter button click event and navigates the user
        /// to the SchedulePage where exam scheduling features are available.
        /// </summary>
        /// <param name="sender">The originator of the event (Enter button).</param>
        /// <param name="e">Event arguments associated with the click event.</param>
        private async void EnterButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SchedulePage());
        }
    }
}
