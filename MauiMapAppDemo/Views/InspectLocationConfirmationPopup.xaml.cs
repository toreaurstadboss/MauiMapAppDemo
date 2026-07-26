using CommunityToolkit.Maui.Views;

namespace MauiMapAppDemo.Views;

public partial class InspectLocationConfirmationPopup : Popup<bool>
{
    private readonly IDispatcherTimer _countdownTimer;
    private int _secondsRemaining = 5;

    public InspectLocationConfirmationPopup(string locationLabel)
    {
        InitializeComponent();

        BindingContext = this;
        LocationLabel = locationLabel;
        CountdownText = "Closing in 5 seconds...";

        _countdownTimer = Dispatcher.CreateTimer();
        _countdownTimer.Interval = TimeSpan.FromSeconds(1);

        _countdownTimer.Tick += OnCountdownTick;
        Opened += OnPopupOpened;
        Closed += OnPopupClosed;
    }

    public string LocationLabel { get; }

    public string CountdownText { get; private set; }

    private void OnPopupOpened(object? sender, EventArgs e)
    {
        _countdownTimer.Start();
    }

    private void OnPopupClosed(object? sender, EventArgs e)
    {
        _countdownTimer.Stop();
        _countdownTimer.Tick -= OnCountdownTick;
        Opened -= OnPopupOpened;
        Closed -= OnPopupClosed;
    }

    private void OnCountdownTick(object? sender, EventArgs e)
    {
        _secondsRemaining--;

        if (_secondsRemaining <= 0)
        {
            _countdownTimer.Stop();
            _ = CloseAsync(false);
            return;
        }

        CountdownText = $"Closing in {_secondsRemaining} seconds...";
        OnPropertyChanged(nameof(CountdownText));
    }

    private async void OnYesClicked(object? sender, EventArgs e)
    {
        _countdownTimer.Stop();
        await CloseAsync(true);
    }

    private async void OnNoClicked(object? sender, EventArgs e)
    {
        _countdownTimer.Stop();
        await CloseAsync(false);
    }
}