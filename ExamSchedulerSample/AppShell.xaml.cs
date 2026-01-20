
using System;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Syncfusion.Maui.Popup;
using Font = Microsoft.Maui.Font;

namespace ExamSchedulerSample
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Keep your theme selection logic
            var currentTheme = Application.Current!.RequestedTheme;
            ThemeSegmentedControl.SelectedIndex = currentTheme == AppTheme.Light ? 0 : 1;
        }

        /// <summary>
        /// Shows a snackbar-style message using Syncfusion SfPopup (bottom-aligned, auto-dismiss).
        /// </summary>
        public static async Task DisplaySnackbarAsync(string message)
        {
            // Build the view that looks like a snackbar
            var label = new Label
            {
                Text = message,
                TextColor = Colors.White,
                FontSize = 14,
                LineBreakMode = LineBreakMode.WordWrap,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                MaxLines = 2
            };

            var body = new Frame
            {
                BackgroundColor = Color.FromArgb("#FF3300"), // your earlier color
                CornerRadius = 12,
                HasShadow = false,
                Padding = new Thickness(16, 10),
                MinimumHeightRequest = 44,
                Content = label
            };

            var popup = new SfPopup();

            // Configure the popup surface via PopupView
            popup.ShowHeader = false;
            popup.ShowFooter = false;
            popup.ContentTemplate = new DataTemplate(() => body);

            var page = Application.Current?.MainPage;
            if (page is null)
                return;

            // Anchor to a View (not Page) -> use page.Content
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                var anchorView = page; // Microsoft.Maui.Controls.View
                if (anchorView is null)
                {
                    // Fallback: center of screen
                    popup.Show();
                }
                else
                {
                    // Bottom-aligned with slight upward offset
                    popup.ShowRelativeToView(anchorView, PopupRelativePosition.AlignBottom, 0, -24);
                }
            });

            // Auto-dismiss after ~3 seconds
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(3));
            }
            finally
            {
                MainThread.BeginInvokeOnMainThread(() => popup.IsOpen = false);
            }
        }

        /// <summary>
        /// Shows a toast-style message using Syncfusion SfPopup (compact, auto-dismiss).
        /// </summary>
        public static async Task DisplayToastAsync(string message)
        {
            var label = new Label
            {
                Text = message,
                TextColor = Colors.White,
                FontSize = 18, // same size you used before
                LineBreakMode = LineBreakMode.WordWrap,
                HorizontalOptions = LayoutOptions.Center,
                VerticalTextAlignment = TextAlignment.Center
            };

            var surface = new Frame
            {
                BackgroundColor = Color.FromArgb("#333333"),
                CornerRadius = 12,
                HasShadow = false,
                Padding = new Thickness(14, 10),
                MinimumHeightRequest = 44,
                Content = label
            };

            var popup = new SfPopup();

            // Configure via PopupView
            popup.ShowHeader = false;
            popup.ShowFooter = false;
            popup.ContentTemplate = new DataTemplate(() => surface);

            var page = Application.Current?.MainPage;
            if (page is null)
                return;

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                var anchorView = page;
                if (anchorView is null)
                {
                    popup.Show();
                }
                else
                {
                    // Toast near the bottom; adjust offset as desired
                    popup.ShowRelativeToView(anchorView, RelativePosition.AlignBottom, 0, -40);
                }
            });

            // Auto-dismiss after ~2.5 seconds
            try
            {
                await Task.Delay(TimeSpan.FromMilliseconds(2500));
            }
            finally
            {
                MainThread.BeginInvokeOnMainThread(() => popup.IsOpen = false);
            }
        }

        // Keep your Syncfusion segmented control theme handler
        private void SfSegmentedControl_SelectionChanged(
            object? sender,
            Syncfusion.Maui.Toolkit.SegmentedControl.SelectionChangedEventArgs e)
        {
            Application.Current!.UserAppTheme = e.NewIndex == 0 ? AppTheme.Light : AppTheme.Dark;
        }
    }
}
