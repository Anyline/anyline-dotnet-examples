using Anyline.Examples.MAUI.Models;
using Microsoft.Maui.Controls.Shapes;

namespace Anyline.Examples.MAUI;

public partial class ResultsPage : ContentPage
{
    /// <summary>
    /// This page is reponsible for displaying the scan results. 
    /// It is built in a generic way to support any scan mode, but in your use-case, it is recommended to work only with a strongly typed object in the MAUI layer.
    /// </summary>
    /// <param name="results"></param>
    /// <param name="scanMode">Object containing the Name of the ScanMode and the JSON config file path (used for re-initializing the ScanView page)</param>
    public ResultsPage(Dictionary<string, Object> results, AnylineScanMode scanMode)
    {
        InitializeComponent();
        ShowResults(results);

        btHome.Clicked += async (s, e) => await Navigation.PopToRootAsync();
        btScanAgain.Clicked += (s, e) =>
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                Navigation.InsertPageBefore(new MyScanningWithAnylinePage(scanMode), this);
                await Navigation.PopAsync();
            });
        };
    }

    private void ShowResults(Dictionary<string, object> results)
    {
        foreach (var result in results)
        {
            if (result.Value is Dictionary<string, object> values)
            {
                slContent.Children.Add(new Label { Text = result.Key, TextColor = Colors.Gray, FontAttributes = FontAttributes.Bold, FontSize = 13 });
                ShowResults(values);
            }
            else
            {
                if (result.Key.ToLower() == "pluginid")
                {
                    slContent.Children.Add(new Label { Text = result.Value.ToString(), TextColor = Color.FromArgb("32ADFF"), FontSize = 20, HorizontalTextAlignment = TextAlignment.Center });
                }
                else
                {
                    slContent.Children.Add(new Label { Text = result.Key, TextColor = Color.FromArgb("32ADFF"), FontSize = 15 });
                    ShowContent(result.Value);
                }
            }
        }
    }

    private void ShowContent(object value)
    {
        if (value is byte[] byteImage)
        {
            var img = new Image()
            {
                Aspect = Aspect.AspectFit,
                HeightRequest = 100,
                Source = ImageSource.FromStream(() => new MemoryStream(byteImage))
            };

            slContent.Children.Add(img);
        }
        else
        {
            slContent.Children.Add(new Label { Text = value.ToString(), TextColor = Color.FromRgb(255, 255, 255), FontAttributes = FontAttributes.Bold, FontSize = 17 });
        }
    }
}
