
using Anyline.Examples.MAUI.Models;
using Anyline.SDK.NET.Common.ScanView.BarcodeOverlays;

namespace Anyline.Examples.MAUI.Views
{
    /// <summary>
    /// This ContentView is rendered natively in each individual platform. 
    /// The scanning results are received in the 'OnResult' action.
    /// </summary>
    internal class AnylineScanningView : ContentView
    {
        public AnylineScanMode ScanMode;
        public Action<object> OnResult;
        public Action OnDisappearing;
        
        public readonly ICommonBarcodeOverlayListener BarcodeOverlayListener = null;

        /// <summary>
        /// Holds the information necessary for the ScanView Initialization and Result processing.
        /// </summary>
        /// <param name="scanMode">Object containing the Name of the ScanMode and the JSON config file path.</param>
        /// <param name="onResultAction">The Action that should be called once the results are available.</param>
        /// <param name="barcodeOverlayListener">The ICommonBarcodeOverlayListener to apply or null.</param>
        public AnylineScanningView(AnylineScanMode scanMode, Action<object> onResultAction, ICommonBarcodeOverlayListener barcodeOverlayListener = null)
        {
            ScanMode = scanMode;
            OnResult = onResultAction;
            BarcodeOverlayListener = barcodeOverlayListener;
            BackgroundColor = Color.FromArgb("00000000");
        }
    }
}