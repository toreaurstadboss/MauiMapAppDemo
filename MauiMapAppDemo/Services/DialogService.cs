using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using MauiMapAppDemo.Views;

namespace MauiMapAppDemo.Services
{

    public class DialogService
    {

        public async Task ShowAlertAsync(string title, string message, string cancel = "OK")
        {
            await Shell.Current.DisplayAlertAsync(
                title,
                message,
                cancel);
        }

        public async Task<string> ShowActionSheetAsync(string title,  string message, params string[] buttons)
        {
            return await Shell.Current.DisplayActionSheetAsync($"{title}\n\n{message}", "OK", null,
                buttons);
        }

        public async Task ShowKartverketInfoPopupAsync(double latitude, double longitude, KartverketPunktResponse? response)
        {
            var popup = new KartverketInfoPopup(latitude, longitude, response);

            await Shell.Current.ShowPopupAsync(
                popup,
                new PopupOptions
                {
                    CanBeDismissedByTappingOutsideOfPopup = true
                });
        }

    }
}
