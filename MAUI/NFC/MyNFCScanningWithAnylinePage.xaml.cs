
using Anyline.Examples.MAUI.Models;
using Anyline.Examples.MAUI.Views;
using Anyline.Examples.MAUI.NFC;

namespace Anyline.Examples.MAUI.NFC;

/// <summary>
/// This is your app's page where Anyline will be integrated.
/// </summary>
public partial class MyNFCScanningWithAnylinePage : ContentPage
{
    private AnylineScanMode _scanMode;
    private MyScanResults _myScanResults;

    /// <summary>
    /// The constructor initializes a new "NFCScanExampleView" (which is rendered natively in Android),
    /// provides a "myResultAction", which will be called once the scanning process is successfully completed,
    /// and a "myErrorAction", which will be called if an error in the process occurs.
    /// </summary>
    /// <param name="scanMode">Object containing the Name of the ScanMode and the JSON config file path (used for initializing the ScanView).</param>
    public MyNFCScanningWithAnylinePage(AnylineScanMode scanMode)
    {
        InitializeComponent();
        _scanMode = scanMode;

        Title = scanMode.Name;

        Action<MyScanResults> myResultAction = (r) =>
        {
            // save the results in a variable to display them after this page appears again
            _myScanResults = r;
        };

        Action<string> myErrorAction = (errorMessage) =>
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("NFC Error", errorMessage, "OK");
            });
        };

        var view = new NFCScanExampleView(scanMode.JSONConfigPath, myResultAction, myErrorAction);

        gridContent.Add(view);
    }

    // Display the NFC results only after we come back to this page
    // (the NFC Scanning activities/pages)
    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (_myScanResults != null)
        {
            ShowNFCResults(_myScanResults, _scanMode);
        }
    }

    /// <summary>
    /// This method is called inside the Action, used to process the Scan Results.
    /// </summary>
    /// <param name="myScanResults">The scan results, coming from the native platform.</param>
    /// <param name="scanMode">Object containing the Name of the ScanMode and the JSON config file path (used for re-initializing the ScanView page).</param>
    private void ShowNFCResults(MyScanResults myScanResults, AnylineScanMode scanMode)
    {
        MainThread.InvokeOnMainThreadAsync(new Action(async () =>
        {
            // Opens the Results Page to display the MRZ and NFC data
            Navigation.InsertPageBefore(new NFCResultsPage(myScanResults, scanMode), Navigation.NavigationStack.Last());
            await Navigation.PopAsync();
        }));
    }
}