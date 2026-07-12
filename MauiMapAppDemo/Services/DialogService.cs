namespace MauiMapAppDemo.Services
{

    public class DialogService
    {

        public async Task ShowAlertAsync(string title, string message, string cancel)
        {
            await Shell.Current.DisplayAlertAsync(
                title,
                message,
                cancel = "OK");
        }

    }
}
