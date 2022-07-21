using Anyline.SDK.NET.iOS;
using CoreGraphics;
using Foundation;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;
using Microsoft.Maui.Controls.Platform;
using UIKit;

namespace Anyline.Examples.MAUI.Platforms.iOS.CustomRenderers
{
    /// <summary>
    /// Currently using this deprecated "PageRenderer" due to some limitations with the usage of the new handlers.
    /// </summary>
    internal class AnylineScanPageRenderer : PageRenderer
    {
        private CGRect frame;
        private bool initialized;

        private ALScanView _scanView;
        ScanResultDelegate _resultDelegate;


        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
            {
                return;
            }

            InitializeAnyline();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            if (!initialized) return;

            StartScanning();
        }

        private void InitializeAnyline()
        {
            NSError error = null;

            if (initialized)
                return;

            try
            {
                // Obtain the JSON config file path from the "AnylineScanPage", defined in the MAUI level.
                string jsonConfigFilePath = (Element as AnylineScanPage).JSONConfigPath.Replace(".json", "") + ".json";

                _resultDelegate = new ScanResultDelegate((Element as AnylineScanPage).OnResult);
                
                // This is the main intialization method that will create our use case depending on the JSON configuration.
                _scanView = ALScanView.ScanViewForFrame(View.Bounds, jsonConfigFilePath, _resultDelegate, out error);

                if (error != null)
                {
                    throw new Exception(error.LocalizedDescription);
                }

                ConnectDelegateToScanPlugin();

                View.AddSubview(_scanView);

                // Pin the leading edge of the scan view to the parent view

                _scanView.TranslatesAutoresizingMaskIntoConstraints = false;

                _scanView.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor).Active = true;
                _scanView.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor).Active = true;
                _scanView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor).Active = true;
                _scanView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor).Active = true;

                _scanView.StartCamera();

                initialized = true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Init Error", e.Message);
            }
        }

        private void StartScanning()
        {
            NSError error = null;
            var success = _scanView.ScanViewPlugin.StartAndReturnError(out error);
            if (!success)
            {
                if (error != null)
                {
                    System.Diagnostics.Debug.WriteLine("Start Scanning Error", error.DebugDescription);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Start Scanning Error", "error is null");
                }
            }
        }

        private void ConnectDelegateToScanPlugin()
        {
            (_scanView.ScanViewPlugin as ALIDScanViewPlugin)?.IdScanPlugin.AddDelegate(_resultDelegate);
            (_scanView.ScanViewPlugin as ALBarcodeScanViewPlugin)?.BarcodeScanPlugin.AddDelegate(_resultDelegate);
            (_scanView.ScanViewPlugin as ALOCRScanViewPlugin)?.OcrScanPlugin.AddDelegate(_resultDelegate);
            (_scanView.ScanViewPlugin as ALMeterScanViewPlugin)?.MeterScanPlugin.AddDelegate(_resultDelegate);
            (_scanView.ScanViewPlugin as ALLicensePlateScanViewPlugin)?.LicensePlateScanPlugin.AddDelegate(_resultDelegate);
            (_scanView.ScanViewPlugin as ALTireScanViewPlugin)?.TireScanPlugin.AddDelegate(_resultDelegate);
            (_scanView.ScanViewPlugin as ALAbstractScanViewPluginComposite)?.AddDelegate(_resultDelegate);
        }

        #region teardown
        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            initialized = false;

            if (_scanView?.ScanViewPlugin == null)
                return;

            NSError error;
            _scanView.ScanViewPlugin.StopAndReturnError(out error);
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            Dispose();
        }

        new void Dispose()
        {
            _scanView?.RemoveFromSuperview();
            _scanView?.Dispose();
            _scanView = null;
            base.Dispose();
        }
        #endregion
    }
}