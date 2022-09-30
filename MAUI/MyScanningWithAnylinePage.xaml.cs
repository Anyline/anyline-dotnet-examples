
using Anyline.Examples.MAUI.Models;
using Anyline.Examples.MAUI.Views;

namespace Anyline.Examples.MAUI;

/// <summary>
/// This is your app's page where Anyline will be integrated.
/// For Android, you can use Anyline as a single View on the page, or together with other views.
/// For iOS, this is only possible when informing the Height and Width of the component. Some extra changes are required in the iOS "AnylineScanViewRenderer".
/// </summary>
public partial class MyScanningWithAnylinePage : ContentPage
{
    /// <summary>
    /// The constructor initializes a new "AnylineScanningView" (which is rendered natively in Android),
    /// and provides a "myResultAction", which will be called once the scanning process is successfully completed.
    /// </summary>
    /// <param name="scanMode">Object containing the Name of the ScanMode and the JSON config file path (used for initializing the ScanView).</param>
    public MyScanningWithAnylinePage(AnylineScanMode scanMode)
    {
        InitializeComponent();  
        Title = scanMode.Name;
        Action<object> myResultAction = (r) =>
        {
            var results = r as Dictionary<string, object>;
            DoSomethingWithResult(results, scanMode);
        };

        // Known Limitation: On iOS, the only way to use the Anyline ScanView alongside other views is by giving it a HeightRequest and a WidthRequest, e.g.:
        //      HeightRequest = 500, WidthRequest = DeviceDisplay.Current.MainDisplayInfo.Width / DeviceDisplay.Current.MainDisplayInfo.Density
        // But this will prevent rotation to work properly. Changes are also required in the iOS Renderer.
        var view = new AnylineScanningView(scanMode.JSONConfigPath, myResultAction);

        //Grid.SetColumnSpan(view, 2);

        gridContent.Add(view, 0, 0);
    }

    /// <summary>
    /// This method is called inside the Action, used to process the Scan Results.
    /// </summary>
    /// <param name="results">The scan results, coming from the native platform.</param>
    /// <param name="scanMode">Object containing the Name of the ScanMode and the JSON config file path (used for re-initializing the ScanView page).</param>
    private void DoSomethingWithResult(Dictionary<string, object> results, AnylineScanMode scanMode)
    {
        Dispatcher.Dispatch(new Action(async () =>
        {
            Navigation.InsertPageBefore(new ResultsPage(results, scanMode), Navigation.NavigationStack.Last());
            await Navigation.PopAsync();
        }));
    }
}