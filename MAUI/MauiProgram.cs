using Anyline.Examples.MAUI.Handlers;


namespace Anyline.Examples.MAUI;

using NFC;
using Views;

#if ANDROID
using Anyline.Examples.MAUI.Platforms.Android.CustomRenderers;
using Anyline.Examples.MAUI.Platforms.Android.NFC;
#endif

#if IOS
using Anyline.Examples.MAUI.Platforms.iOS.CustomRenderers;
using Anyline.Examples.MAUI.Platforms.iOS.NFC;
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
                handlers.AddHandler(typeof(NFCScanExampleView), typeof(NFCScanningViewRenderer));
            })
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        return builder.Build();
    }
}
