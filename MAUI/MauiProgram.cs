using Anyline.Examples.MAUI.Handlers;


namespace Anyline.Examples.MAUI;

using Views;

#if ANDROID
using Anyline.Examples.MAUI.Platforms.Android.CustomRenderers;
#endif

#if IOS
using Anyline.Examples.MAUI.Platforms.iOS.CustomRenderers;
#endif

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureMauiHandlers(handlers =>
            {
                //handlers.AddHandler(typeof(AnylineView), typeof(AnylineHandler));
                handlers.AddHandler(typeof(AnylineScanningView), typeof(AnylineScanningViewRenderer));
            })
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        return builder.Build();
    }
}
