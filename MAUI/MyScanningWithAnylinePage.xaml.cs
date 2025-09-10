
using Anyline.Examples.MAUI.Models;
using Anyline.Examples.MAUI.Models.BarcodeOverlays;
using Anyline.Examples.MAUI.Views;
using Anyline.SDK.NET.Common;

namespace Anyline.Examples.MAUI;

/// <summary>
/// This is your app's page where Anyline will be integrated.
/// </summary>
public partial class MyScanningWithAnylinePage : ContentPage
{
    private readonly AnylineScanMode ScanMode;
    private readonly CommonBarcodeOverlayListenerImpl barcodeOverlayListener = null;
    private readonly AnylineScanningView _anylineScanningView;
    
    /// <summary>
    /// The constructor initializes a new "AnylineScanningView" (which is rendered natively in Android & iOS),
    /// and provides a "myResultAction", which will be called once the scanning process is successfully completed.
    /// </summary>
    /// <param name="scanMode">Object containing the Name of the ScanMode and the JSON config file path (used for initializing the ScanView).</param>
    public MyScanningWithAnylinePage(AnylineScanMode scanMode)
    {
        ScanMode = scanMode;
        
        InitializeComponent();  
        Title = scanMode.Name;
        Action<object> myResultAction = (r) =>
        {
            var results = r as ScanResult[];
            DoSomethingWithResult(results, scanMode);
        };
        
        if (scanMode.ApplyBarcodeOverlays)
        {
            barcodeOverlayListener = new CommonBarcodeOverlayListenerImpl();
            
            NavigationPage.SetHasBackButton(this, false);
            ToolbarItems.Add(new ToolbarItem
            {
                Text = "View Selected",
                Command = new Command(async () =>
                {
                    var selectedBarcodes = barcodeOverlayListener.GetSelectedBarcodes();
                    if (selectedBarcodes.Count > 0)
                    {
                        Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(300), async void () =>
                        {
                            Navigation.InsertPageBefore(new ResultsPage(selectedBarcodes.ToArray(), ScanMode), Navigation.NavigationStack.Last());
                            await Navigation.PopAsync();
                        });
                    }                    
                })
            });
        }        
        _anylineScanningView = new AnylineScanningView(scanMode, myResultAction, barcodeOverlayListener);

        gridContent.Add(_anylineScanningView);
    }

    /// <summary>
    /// This method is called inside the Action, used to process the Scan Results.
    /// </summary>
    /// <param name="scanResults">The scan results, translated into common ScanResult from the native platform.</param>
    /// <param name="scanMode">Object containing the Name of the ScanMode and the JSON config file path (used for re-initializing the ScanView page).</param>
    private void DoSomethingWithResult(ScanResult[] scanResults, AnylineScanMode scanMode)
    {
        if (!scanMode.IsContinuous())
        {
            Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(300), async void () =>
            {
                Navigation.InsertPageBefore(new ResultsPage(scanResults, scanMode), Navigation.NavigationStack.Last());
                await Navigation.PopAsync();
            });
        }
    }

    protected override void OnDisappearing()
    {
        _anylineScanningView.OnDisappearing?.Invoke();
        base.OnDisappearing();
    }
    
}