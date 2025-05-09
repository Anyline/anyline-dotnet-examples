
using Anyline.Examples.MAUI.Models;
using Anyline.Examples.MAUI.Views;

namespace Anyline.Examples.MAUI;

/// <summary>
/// This is your app's page where Anyline will be integrated.
/// </summary>
public partial class MyScanningWithAnylinePage : ContentPage
{
    int _resultCount = 0;
    /// <summary>
    /// The constructor initializes a new "AnylineScanningView" (which is rendered natively in Android & iOS),
    /// and provides a "myResultAction", which will be called once the scanning process is successfully completed.
    /// </summary>
    /// <param name="scanMode">Object containing the Name of the ScanMode and the JSON config file path (used for initializing the ScanView).</param>
    public MyScanningWithAnylinePage(AnylineScanMode scanMode)
    {
        InitializeComponent();  
        Title = scanMode.Name;
        Action<object> myResultAction = (r) =>
        {
            var results = r as Lazy<Dictionary<string, object>>;
            DoSomethingWithResult(results, scanMode);
        };

        var view = new AnylineScanningView(scanMode.JSONConfigPath, myResultAction);

        gridContent.Add(view);
    }

    /// <summary>
    /// This method is called inside the Action, used to process the Scan Results.
    /// </summary>
    /// <param name="results">The scan results, coming from the native platform.</param>
    /// <param name="scanMode">Object containing the Name of the ScanMode and the JSON config file path (used for re-initializing the ScanView page).</param>
    private void DoSomethingWithResult(Lazy<Dictionary<string, object>> results, AnylineScanMode scanMode)
    {
        if (scanMode.IsContinuous())
        {
            _resultCount++;
            Title = scanMode.Name + " (" + _resultCount + ")";
        }
        else
        {
            Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(300), async void () =>
            {
                Navigation.InsertPageBefore(new ResultsPage(results.Value, scanMode), Navigation.NavigationStack.Last());
                await Navigation.PopAsync();
            });
        }
    }
}