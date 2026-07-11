using MauiMapAppDemo.Pages;
using MauiMapAppDemo.Services;

namespace MauiMapAppDemo
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        private readonly OpenTopoService _openTopoService;

        public MainPage(OpenTopoService openTopoService)
        {
            InitializeComponent();
            Navigation.PushAsync(new MapsDemo(openTopoService)); //just redirect to maps demo
            this._openTopoService = openTopoService;
        }

        private void OnNavigateToMapsButtonClicked(object? sender, EventArgs e)
        {
            Navigation.PushAsync(new MapsDemo(_openTopoService));
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
