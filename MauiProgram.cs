using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MauiMapAppDemo
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiMaps()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            //NOTE : This demo app code only will show App in LocalDEV since we use USER SECRETS ! 

            builder.Configuration.AddUserSecrets<App>(); //only set up user secrets when we are doing local debugging
#endif 

            string azureMapsKey = builder.Configuration["AzureMapsKey"] ?? string.Empty;
            builder.ConfigureEssentials(essentials => essentials.UseMapServiceToken(azureMapsKey));

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
