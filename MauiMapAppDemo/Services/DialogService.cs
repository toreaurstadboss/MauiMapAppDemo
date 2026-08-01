using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;
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

        public async Task<string> ShowActionSheetAsync(string title, string message, params string[] buttons)
        {
            return await Shell.Current.DisplayActionSheetAsync($"{title}\n\n{message}", "OK", null,
                buttons);
        }

        public async Task<bool> ShowInspectLocationConfirmationPopupAsync(string title)
        {
            var popup = new InspectLocationConfirmationPopup(title);

            var result = await Shell.Current.ShowPopupAsync<bool>(
                popup,
                new PopupOptions
                {
                    CanBeDismissedByTappingOutsideOfPopup = false
                });

            return result.Result;
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
