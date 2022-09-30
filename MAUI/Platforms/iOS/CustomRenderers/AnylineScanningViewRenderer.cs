using Anyline.Examples.MAUI.Views;
using Anyline.SDK.NET.iOS;
using CoreGraphics;
using Foundation;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform;
using UIKit;

namespace Anyline.Examples.MAUI.Platforms.iOS.CustomRenderers
{
    /* 
     * Known issues:
     * It is not possible to read the "Bounds" of the View if it was not assigned the HeightRequest and WidthRequest properties 
     *  -> No possibility to use the ScanView alongside other views without those values
     *  -> If the values were informed, using the ScanView alongside other views is possible, but then Rotation will not work properly
     */

    /// <summary>
    /// This class is responsible for rendering the Anyline ScanView natively.
    /// </summary>
    internal class AnylineScanningViewRenderer : ViewRenderer
    {
        private ALScanView _scanView;
        private ScanResultDelegate _resultDelegate;

        public override void WillMoveToWindow(UIWindow window)
        {
            base.WillMoveToWindow(window);
            if (window != null)
                InitializeAnylineScanView(window);
            else
                DisposeAnyline();
        }

        private void InitializeAnylineScanView(UIWindow window)
        {
            NSError error = null;

            try
            {
                // Obtain the JSON config file path, defined in the MAUI level.
                string jsonConfigFilePath = (Element as AnylineScanningView).JSONConfigPath.Replace(".json", "");

                var configPath = NSBundle.MainBundle.PathForResource(jsonConfigFilePath, @"json");

                // This is the main intialization method that will create our use case depending on the JSON configuration.
                _resultDelegate = new ScanResultDelegate((Element as AnylineScanningView).OnResult);

                CGRect scanViewBounds = window.Screen.Bounds;
                // If the AnylineScanView was manually given Height and Width values, use its own Bounds instead
                if (Bounds != new CGRect())
                    scanViewBounds = Bounds;

                _scanView = ALScanView.ScanViewForFrame(scanViewBounds, configPath, _resultDelegate, out error);

                if (error != null)
                {
                    throw new Exception(error.LocalizedDescription);
                }

                ConnectDelegateToScanPlugin();

                Add(_scanView);

                _scanView.StartCamera();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("EXCEPTION: " + e.Message);
            }
        }

        /// <summary>
        /// This method is called after the _scanView is added.
        /// </summary>
        /// <param name="uiview">The _scanView itself</param>
        public override void SubviewAdded(UIView uiview)
        {
            base.SubviewAdded(uiview);

            // To allow rotation, pin the edge of the scan view to the parent view:
            _scanView.TranslatesAutoresizingMaskIntoConstraints = false;

            _scanView.LeadingAnchor.ConstraintEqualTo(Superview.LeadingAnchor).Active = true;
            _scanView.TrailingAnchor.ConstraintEqualTo(Superview.TrailingAnchor).Active = true;
            _scanView.TopAnchor.ConstraintEqualTo(Superview.SafeAreaLayoutGuide.TopAnchor).Active = true;
            _scanView.BottomAnchor.ConstraintEqualTo(Superview.BottomAnchor).Active = true;

            // Start scanning!
            BeginInvokeOnMainThread(() => StartAnylineScanner());
        }

        private void StartAnylineScanner()
        {
            NSError error = null;
            var success = _scanView.ScanViewPlugin.StartAndReturnError(out error);
            if (!success)
            {
                if (error != null)
                {
                    System.Diagnostics.Debug.WriteLine(error.DebugDescription);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("error is null");
                }
            }
        }

        protected void DisposeAnyline()
        {
            if (_scanView != null && _scanView.ScanViewPlugin != null)
            {
                _scanView.ScanViewPlugin.StopAndReturnError(out NSError error);

                if (error != null)
                    System.Diagnostics.Debug.WriteLine(error.DebugDescription);
            }

            _scanView?.Dispose();
            _scanView?.RemoveFromSuperview();
            _scanView = null;
        }

        private void ConnectDelegateToScanPlugin()
        {
            (_scanView.ScanViewPlugin as ALIDScanViewPlugin)?.IdScanPlugin.AddDelegate(_resultDelegate);
            (_scanView.ScanViewPlugin as ALBarcodeScanViewPlugin)?.BarcodeScanPlugin.AddDelegate(_resultDelegate);
            (_scanView.ScanViewPlugin as ALOCRScanViewPlugin)?.OcrScanPlugin.AddDelegate(_resultDelegate);
            (_scanView.ScanViewPlugin as ALMeterScanViewPlugin)?.MeterScanPlugin.AddDelegate(_resultDelegate);
            (_scanView.ScanViewPlugin as ALDocumentScanViewPlugin)?.DocumentScanPlugin.AddDelegate(_resultDelegate);
            (_scanView.ScanViewPlugin as ALLicensePlateScanViewPlugin)?.LicensePlateScanPlugin.AddDelegate(_resultDelegate);
            // add listener to the composite as a whole (to get the information once all the results are available)
            (_scanView.ScanViewPlugin as ALAbstractScanViewPluginComposite)?.AddDelegate(_resultDelegate);

            //// OR 

            //// add individual listeners (in case you need to listen to partial results and interrupt the workflow)
            //// -> in this case, remember to call "scanView.ScanViewPlugin.StopAndReturnError(out error)" after the result to stop scanning.

            //var parallelComposite = (_scanView.ScanViewPlugin as ALParallelScanViewPluginComposite);
            //if (parallelComposite != null)
            //{
            //    foreach (ALAbstractScanViewPlugin item in parallelComposite.ChildPlugins.Values)
            //    {
            //        if (item is ALMeterScanViewPlugin meterScanViewPlugin)
            //        {
            //            meterScanViewPlugin.MeterScanPlugin.AddDelegate(_resultDelegate);
            //        }
            //        else if (item is ALBarcodeScanViewPlugin barcodeScanViewPlugin)
            //        {
            //            barcodeScanViewPlugin.BarcodeScanPlugin.AddDelegate(_resultDelegate);
            //        }
            //    }
            //}
        }
    }
}
