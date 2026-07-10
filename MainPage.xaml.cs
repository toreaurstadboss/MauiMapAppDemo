using MauiMapAppDemo.Pages;

namespace MauiMapAppDemo
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
            Navigation.PushAsync(new MapsDemo()); //just redirect to maps demo

        }

        private void OnNavigateToMapsButtonClicked(object? sender, EventArgs e)
        {
            Navigation.PushAsync(new MapsDemo());
        }

        private void OnCounterClicked(object? sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }
}
