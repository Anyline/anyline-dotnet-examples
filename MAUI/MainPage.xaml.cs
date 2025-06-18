using Anyline.Examples.MAUI.Models;
using Microsoft.Maui.Controls.Shapes;

namespace Anyline.Examples.MAUI;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        NavigationPage.SetBackButtonTitle(this, "Home");

        // (this license key should be, ideally, securely fetched from your back-end server, a secret manager/provider, or obfuscated in the final app)

        string licenseErrorMessage = null;

        // Initializes the Anyline SDK natively in each platform and gets the result of the initialization back
        AnylineSDKService anylineSDKService = new AnylineSDKService();
        bool isAnylineInitialized = anylineSDKService.SetupWithLicenseKey(licenseKey, out licenseErrorMessage);

        if (isAnylineInitialized)
        {
            // Load and present Anyline's tech capabilities list
            foreach (var group in AnylineScanModes.GetAnylineScanModesGroupedList())
            {
                slScanModes.Children.Add(new Line { BackgroundColor = Colors.White, HeightRequest = 0.5, Margin = new Thickness(0, 10) });
                slScanModes.Children.Add(new Label { Text = group.Name, TextColor = Colors.White, FontSize = 20, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, Margin = 7 });
                foreach (var scanMode in group)
                {
                    var btScan = new Button() { Text = scanMode.Name, BackgroundColor = Color.FromArgb("32ADFF"), TextColor = Colors.White, Padding = 15, Margin = 10 };
                    btScan.Clicked += BtScan_Clicked;
                    btScan.CommandParameter = scanMode;
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

        ShowAnylineSDKVersion(anylineSDKService);
    }

    private async void BtScan_Clicked(object sender, EventArgs e)
    {
        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
        {
            await Permissions.RequestAsync<Permissions.Camera>();
        }

        var btScan = (sender as Button);
        btScan.IsEnabled = false;
        AnylineScanMode scanMode = btScan.CommandParameter as AnylineScanMode;

        await Navigation.PushAsync(new MyScanningWithAnylinePage(scanMode));

        btScan.IsEnabled = true;
    }

    private void ShowAnylineSDKVersion(AnylineSDKService anylineSDKService)
    {
        slScanModes.Children.Add(new Label { Text = $"Anyline Native SDK Version: {anylineSDKService.GetSDKVersion()}", FontSize = 10, Margin = 10, TextColor = Colors.White });
        slScanModes.Children.Add(new Label { Text = $"Anyline Plugin Version: {anylineSDKService.GetPluginVersion()}", FontSize = 10, Margin = 10, TextColor = Colors.White });
        slScanModes.Children.Add(new Label { Text = $"App build: {GetAppVersion()}", FontSize = 10, Margin = 10, TextColor = Colors.White });
    }
    private string GetAppVersion()
    {
        return typeof(MainPage).Assembly.GetName().Version.ToString();
    }

}

