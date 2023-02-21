using Anyline.Examples.MAUI.Models;
using Anyline.Examples.MAUI.NFC;
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
        // (this license key should be, ideally, securely fetched from your back-end server, a secret manager/provider, or obfuscated in the final app)
        string licenseKey = "ewogICJsaWNlbnNlS2V5VmVyc2lvbiI6ICIzLjAiLAogICJkZWJ1Z1JlcG9ydGluZyI6ICJwaW5nIiwKICAibWFqb3JWZXJzaW9uIjogIjM3IiwKICAic2NvcGUiOiBbCiAgICAiQUxMIgogIF0sCiAgIm1heERheXNOb3RSZXBvcnRlZCI6IDUsCiAgImFkdmFuY2VkQmFyY29kZSI6IHRydWUsCiAgIm11bHRpQmFyY29kZSI6IHRydWUsCiAgInN1cHBvcnRlZEJhcmNvZGVGb3JtYXRzIjogWwogICAgIkFMTCIKICBdLAogICJwbGF0Zm9ybSI6IFsKICAgICJpT1MiLAogICAgIkFuZHJvaWQiCiAgXSwKICAic2hvd1dhdGVybWFyayI6IHRydWUsCiAgInRvbGVyYW5jZURheXMiOiAzMCwKICAidmFsaWQiOiAiMjAyMy0wNi0zMCIsCiAgImlvc0lkZW50aWZpZXIiOiBbCiAgICAiY29tLmFueWxpbmUueGFtYXJpbi5leGFtcGxlcyIsCiAgICAiY29tLmFueWxpbmUueGFtYXJpbi5mb3Jtcy5leGFtcGxlcyIsCiAgICAiY29tLmFueWxpbmUuZXhhbXBsZXMiLAogICAgImNvbS5hbnlsaW5lLm1hdWkuZXhhbXBsZXMiCiAgXSwKICAiYW5kcm9pZElkZW50aWZpZXIiOiBbCiAgICAiY29tLmFueWxpbmUueGFtYXJpbi5leGFtcGxlcyIsCiAgICAiY29tLmFueWxpbmUueGFtYXJpbi5mb3Jtcy5leGFtcGxlcyIsCiAgICAiY29tLmFueWxpbmUuZXhhbXBsZXMiLAogICAgImNvbS5hbnlsaW5lLm1hdWkuZXhhbXBsZXMiCiAgXQp9CnhFU0JoNVhZSGQyMXh3aHkvaUhsNVdYd1NmZDFscytqQ0VST2x2bGRzam1CV1BpWG44MzloZnZCdG00Y0Qyd3VEdVBHeDNMZXZNcUlPOVhxaXJ3Q256Z1hWd00rZ3hZU2k2N21aazJPMzcyQmlRcE1QQU5mUjFEQTBwcGZPSVN4V09ocjcwVXViNWJicnhYREVmblZPU0svYXFzb0Z2YzVEdDlTNWlmeE5POXV3dVY3Q3RIcjBKZ2NpcmU1UkdzS0xocmEyUTE5TVBDT1hsb3U0MlJadG9JN1dSMnZ6NUR6eUhkUDhGRjJUVlF0TWRiNGZxS1lNbFB3TXRKYUl5OEZtMkRTdmp6QlBCU0RRQUo3SVZMM2R6NEZ4MnhXQnBvTEN6YXNDdG4rc21vakJPYUIwc1NkckFBWFU5RFEwa1BIUjhUMW5XSmZMQ3JOeml2ZUdVK1RjZz09";

        string licenseErrorMessage = null;

        // Initializes the Anyline SDK natively in each platform and gets the result of the initialization back
        bool isAnylineInitialized = new AnylineSDKService().SetupWithLicenseKey(licenseKey, out licenseErrorMessage);

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

        if (scanMode.Name == "Scan NFC of Passports")
        {
            await Navigation.PushAsync(new MyNFCScanningWithAnylinePage(scanMode));
        }
        else
        {
            await Navigation.PushAsync(new MyScanningWithAnylinePage(scanMode));
        }

        (sender as Button).IsEnabled = true;
    }

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

