using MauiMapAppDemo.Pages;
using MauiMapAppDemo.Services;

namespace MauiMapAppDemo
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        private readonly OpenTopoService _openTopoService;
        private readonly GeocodingService _geocodingService;
        private readonly DialogService _dialogService;

        public MainPage(OpenTopoService openTopoService, GeocodingService geocodingService, DialogService dialogService)
        {
            InitializeComponent();
            Navigation.PushAsync(new MapsDemo(openTopoService, geocodingService, dialogService)); //just redirect to maps demo
            _openTopoService = openTopoService;
            _geocodingService = geocodingService;
            _dialogService = dialogService;
        }

        private void OnNavigateToMapsButtonClicked(object? sender, EventArgs e)
        {
            Navigation.PushAsync(new MapsDemo(_openTopoService, _geocodingService, _dialogService));
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
