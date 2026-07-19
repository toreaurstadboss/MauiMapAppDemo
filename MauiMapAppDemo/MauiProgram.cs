using CommunityToolkit.Maui;
using MauiMapAppDemo.Behaviors;
using MauiMapAppDemo.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Maui;
using Microsoft.Maui.Maps.Handlers;
using Microsoft.Maui.Storage;

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

#if ANDROID
            MapPinHandler.Mapper.AppendToMapping("MeasurementPinIcons", (handler, mapPin) =>
            {
                if (mapPin is not MeasurementPin measurementPin)
                {
                    return;
                }

                var resourceId = GetDrawableResourceId(measurementPin.IconResourceName);
                if (resourceId == 0)
                {
                    return;
                }

                handler.PlatformView.SetIcon(Android.Gms.Maps.Model.BitmapDescriptorFactory.FromResource(resourceId));
            });
#endif

#if DEBUG
            //NOTE : This demo app code only will show App in LocalDEV since we use USER SECRETS ! 

            builder.Configuration.AddUserSecrets<App>(); //only set up user secrets when we are doing local debugging
#endif 

            using (var appSettingsStream = FileSystem.OpenAppPackageFileAsync("appsettings.json").GetAwaiter().GetResult())
            {
                builder.Configuration.AddJsonStream(appSettingsStream);
            }

            string azureMapsKey = builder.Configuration["AzureMapsKey"] ?? string.Empty;
            builder.ConfigureEssentials(essentials => essentials.UseMapServiceToken(azureMapsKey));

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<OpenTopoService>();
            builder.Services.AddSingleton<GoogleElevationService>();
            builder.Services.AddSingleton<IElevationService>(sp =>
            {
                var elevationProvider = builder.Configuration["ElevationProvider"] ?? "OpenTopo";

                return elevationProvider.Equals("Google", StringComparison.OrdinalIgnoreCase)
                    ? sp.GetRequiredService<GoogleElevationService>()
                    : sp.GetRequiredService<OpenTopoService>();
            });
            builder.Services.AddSingleton<GeocodingService>();
            builder.Services.AddSingleton<DialogService>();

            return builder.Build();
        }

#if ANDROID
        private static int GetDrawableResourceId(string resourceName)
        {
            if (string.IsNullOrWhiteSpace(resourceName))
            {
                return 0;
            }

            var field = typeof(Resource.Drawable).GetField(resourceName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.IgnoreCase);
            if (field?.GetValue(null) is int resourceId)
            {
                return resourceId;
            }

            return 0;
        }
#endif
    }
}
