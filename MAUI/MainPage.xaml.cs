using Anyline.Examples.MAUI.Models;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.Shapes;
using System.Reflection;

namespace Anyline.Examples.MAUI;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        NavigationPage.SetBackButtonTitle(this, "Home");

        // YOUR LICENSE KEY HERE
        // (this license key should, ideally, be securely fetched from your back-end server or a secret manage/provider)
        string licenseKey = "ewogICJsaWNlbnNlS2V5VmVyc2lvbiI6ICIzLjAiLAogICJkZWJ1Z1JlcG9ydGluZyI6ICJwaW5nIiwKICAibWFqb3JWZXJzaW9uIjogIjM3IiwKICAic2NvcGUiOiBbCiAgICAiQUxMIgogIF0sCiAgIm1heERheXNOb3RSZXBvcnRlZCI6IDUsCiAgImFkdmFuY2VkQmFyY29kZSI6IHRydWUsCiAgIm11bHRpQmFyY29kZSI6IHRydWUsCiAgInN1cHBvcnRlZEJhcmNvZGVGb3JtYXRzIjogWwogICAgIkFMTCIKICBdLAogICJwbGF0Zm9ybSI6IFsKICAgICJpT1MiLAogICAgIkFuZHJvaWQiCiAgXSwKICAic2hvd1dhdGVybWFyayI6IHRydWUsCiAgInRvbGVyYW5jZURheXMiOiAzMCwKICAidmFsaWQiOiAiMjAyMi0xMi0zMSIsCiAgImlvc0lkZW50aWZpZXIiOiBbCiAgICAiY29tLmFueWxpbmUueGFtYXJpbi5leGFtcGxlcyIsCiAgICAiY29tLmFueWxpbmUueGFtYXJpbi5mb3Jtcy5leGFtcGxlcyIsCiAgICAiY29tLmFueWxpbmUuZXhhbXBsZXMiCiAgXSwKICAiYW5kcm9pZElkZW50aWZpZXIiOiBbCiAgICAiY29tLmFueWxpbmUueGFtYXJpbi5leGFtcGxlcyIsCiAgICAiY29tLmFueWxpbmUueGFtYXJpbi5mb3Jtcy5leGFtcGxlcyIsCiAgICAiY29tLmFueWxpbmUuZXhhbXBsZXMiCiAgXQp9CnRad29IWnlXZmtYV1FldkRBUWdiNUYzQm1xVU9mOWQ2a3Vma0tsY1k0OU1CQWkybXZNUGI3N3JaRkhCeEJ1YUZjTmNrckJXbm83Yjl2U2RWWGNpdlQxcUx0MGtGK1BTMDlBb014alBCWjM3TllnQU5FTCtsdWF6UmhjVWJscmE2ek52UnpCdGhyblpPMy85WmVhZ0JYdTNCWFF3b0Vrc3p3TzJFVndTY0krNEdrb1hNTjFFU2ExL0YyNUhmMlBSay8yUmpvam9YeGdwR0hQVnJXcjRwUG03WlI4ZW1rUUtRU3N1U3NXTjdpMldsUFd0ekNuOU5HcjhvMWxxOUpEU1BvY3NmTXRqc2xwNjliM3Bibk9VR0k5dnFUUkdQZ0hZSktUSGVDVFZVdzlTWkZCb3psTFN4ZEJnalZWSUk1QW1xTTZRMjV1TVpvU044N3NWanhTaTE4Zz09";

        string licenseErrorMessage = null;

        // Initializes the Anyline SDK natively in each platform and get the result of the initialization back
        bool isAnylineInitialized = new AnylineSDKService().SetupWithLicenseKey(licenseKey, out licenseErrorMessage);

        if (isAnylineInitialized)
        {
            // Not using the CollectionView as it currently does not work properly on iOS: https://github.com/dotnet/maui/issues/6605
            //cvScanModes.ItemsSource = AnylineScanModes.GetAnylineScanModesList();

            // ** Still, MAUI has some major ScrollView height calculation issues on iOS compared to Xamarin Forms, so even adding the buttons dynamically might not work on some devices. **

            // Load and present Anyline's tech capabilities list
            foreach (var group in AnylineScanModes.GetAnylineScanModesGroupedList())
            {
                slScanModes.Children.Add(new Line { BackgroundColor = Colors.White, HeightRequest = 0.5, Margin = new Thickness(0, 10) });
                slScanModes.Children.Add(new Label { Text = group.Name, TextColor = Colors.White, FontSize = 20, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, Margin = 7 });
                foreach (var scanMode in group)
                {
                    var btScan = new Button() { Text = scanMode.Name, BackgroundColor = Color.FromArgb("32ADFF"), TextColor = Colors.White, Padding = 15, Margin = 10 };
                    btScan.Clicked += BtScan_Clicked;
                    btScan.ClassId = scanMode.Name + ":" + scanMode.JSONConfigPath;
                    slScanModes.Children.Add(btScan);
                    slScanModes.Add(new Label { Text = licenseErrorMessage, FontSize = 14, TextColor = Colors.White, HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand });
                }
            }
        }
        else
        {
            // If the Anyline SDK initialization failed, prevent the use of the SDK
            slScanModes.Add(new Label { Text = licenseErrorMessage, FontSize = 14, TextColor = Colors.White, HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand });
        }

        ShowAnylineSDKVersion();
    }

    private async void BtScan_Clicked(object sender, EventArgs e)
    {
        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
        {
            await Permissions.RequestAsync<Permissions.Camera>();
        }

        (sender as Button).IsEnabled = false;
        string classId = ((Button)sender).ClassId;
        string[] name_config = classId.Split(":");

        AnylineScanMode scanMode = new AnylineScanMode(name_config[0], name_config[1], string.Empty);

#if IOS
        // On iOS, using the new ViewRender still presents many undesired behaviours and issues,
        // therefore, we are still using the deprecate PageRenderer, for now.
        await Navigation.PushAsync(new AnylineScanPage(scanMode));
#elif ANDROID
       // On Android, the new ViewRenderer can already be used correctly,
       // while the deprecated PageRenderer does not work at all, regardless of the Anyline SDK.
       await Navigation.PushAsync(new MyScanningWithAnylinePage(scanMode));
#endif

        (sender as Button).IsEnabled = true;
    }

    //    private async void cvScanModes_SelectionChanged(object sender, SelectionChangedEventArgs e)
    //    {

    //        if (e.CurrentSelection != e.PreviousSelection)
    //        {
    //            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
    //            if (status != PermissionStatus.Granted)
    //            {
    //                await Permissions.RequestAsync<Permissions.Camera>();
    //            }

    //            var scanMode = e.CurrentSelection.FirstOrDefault() as AnylineScanMode;

    //            if (scanMode != null)
    //#if ANDROID
    //                await Navigation.PushAsync(new MyScanningWithAnylinePage(scanMode));
    //#elif IOS
    //                await Navigation.PushAsync(new AnylineScanPage(scanMode));
    //#endif
    //            //cvScanModes.SelectedItem = null;
    //        }
    //    }

    private void ShowAnylineSDKVersion()
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var assembly = assemblies.Where(x => x.FullName.StartsWith("Anyline.SDK.NET")).FirstOrDefault();
        if (assembly != null)
        {
            Version ver = assembly.GetName().Version;
            slScanModes.Children.Add(new Label { Text = $"Anyline SDK Version: {ver}", FontSize = 10, Margin = 10, TextColor = Colors.White });
            slScanModes.Children.Add(new Label { Text = $"App build: {GetAppVersion()}", FontSize = 10, Margin = 10, TextColor = Colors.White });
        }
    }
    private string GetAppVersion()
    {
        return typeof(MainPage).Assembly.GetName().Version.ToString();
    }

}

