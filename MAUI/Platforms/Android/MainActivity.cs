using Android.App;
using Android.Content.PM;
using Android.OS;

namespace Anyline.Examples.MAUI;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, EnableOnBackInvokedCallback = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    public static Activity Instance;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        Instance = this;
    }
}
