using MauiMapAppDemo.Pages;
using MauiMapAppDemo.Services;

namespace MauiMapAppDemo
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        private readonly OpenTopoService _openTopoService;
        private readonly GeocodingService _geocodingService;

        public MainPage(OpenTopoService openTopoService, GeocodingService geocodingService)
        {
            InitializeComponent();
            Navigation.PushAsync(new MapsDemo(openTopoService, geocodingService)); //just redirect to maps demo
            _openTopoService = openTopoService;
            _geocodingService = geocodingService;
        }

        private void OnNavigateToMapsButtonClicked(object? sender, EventArgs e)
        {
            Navigation.PushAsync(new MapsDemo(_openTopoService, _geocodingService));
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
