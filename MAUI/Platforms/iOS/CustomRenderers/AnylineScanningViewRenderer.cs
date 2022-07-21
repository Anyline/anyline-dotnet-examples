using Anyline.Examples.MAUI.Views;
using Anyline.SDK.NET.iOS;
using Foundation;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform;
using UIKit;

namespace Anyline.Examples.MAUI.Platforms.iOS.CustomRenderers
{
    /* 
     * This Renderer is currently not in use.
     * Current issues:
     * - Cutouts are not correctly positioned vertically
     * - Rotation does not work properly
     * - Not able to read the "Bounds" of the View (just the page) -> No possibility to use it the ScanView as a "ContentView", just as a "Page" (fullscreen)
     */

    internal class AnylineScanningViewRenderer : ViewRenderer
    {
        private bool initialized;
        private ALScanView _scanView;
        ScanResultDelegate _resultDelegate;

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || Element == null)
            {
                return;
            }
        }

        private void InitializeAnyline(UIWindow window)
        {
            NSError error = null;

            if (initialized)
                return;

            try
            {
                string file = (Element as AnylineScanningView).JSONConfigPath.Replace(".json", "");

                var configPath = NSBundle.MainBundle.PathForResource(file, @"json");

                System.Diagnostics.Debug.WriteLine("Config PATH: " + configPath);

                // This is the main intialization method that will create our use case depending on the JSON configuration.
                _resultDelegate = new ScanResultDelegate((Element as AnylineScanningView).OnResult);


                //_scanView = ALScanView.ScanViewForFrame(frameSize, configPath, _resultDelegate, out error);

                _scanView = ALScanView.ScanViewForFrame(window.Screen.Bounds, configPath, _resultDelegate, out error);

                //NSError jsonError = null;

                //NSData jsonData = NSData.FromString("{  \"camera\": {    \"captureResolution\": \"720\"  },  \"flash\": {    \"mode\": \"manual\",    \"alignment\": \"bottom_right\"  },  \"viewPlugin\": {    \"plugin\": {      \"id\": \"USNR_ID\",      \"ocrPlugin\": {        \"ocrConfig\": {}      },      \"delayStartScanTime\": 1000    },    \"cutoutConfig\": {      \"style\": \"rect\",      \"width\": 720,      \"alignment\": \"top_half\",      \"maxWidthPercent\": \"80%\",      \"ratioFromSize\": {        \"width\": 720,        \"height\": 144      },      \"strokeWidth\": 2,      \"strokeColor\": \"FFFFFF\",      \"cornerRadius\": 4,      \"outerColor\": \"000000\",      \"outerAlpha\": 0.5,      \"feedbackStrokeColor\": \"0099FF\",      \"offset\": {        \"x\": 0,        \"y\": -15      }    },    \"scanFeedback\": {      \"style\": \"CONTOUR_RECT\",      \"strokeColor\": \"0099FF\",      \"fillColor\": \"220099FF\",      \"beepOnResult\": true,      \"vibrateOnResult\": true,      \"blinkAnimationOnResult\": true    },    \"cancelOnResult\": true  }}", NSStringEncoding.UTF8);
                //NSDictionary myDictionary = (NSDictionary)NSJsonSerialization.Deserialize(jsonData, NSJsonReadingOptions.FragmentsAllowed, out jsonError);

                //// This is the main intialization method that will create our use case depending on the JSON configuration.
                //_resultDelegate = new ScanResultDelegate((Element as MyScanningView).ShowResultsAction);
                //_scanView = ALScanView.ScanViewForFrame(new CGRect(0, 0, 700, 700), myDictionary, _resultDelegate, out error);

                if (error != null)
                {
                    throw new Exception(error.LocalizedDescription);
                }

                ConnectDelegateToScanPlugin();

                // Pin the leading edge of the scan view to the parent view

                _scanView.TranslatesAutoresizingMaskIntoConstraints = true;

                //_scanView.LeadingAnchor.ConstraintEqualTo(LeadingAnchor).Active = true;
                //_scanView.TrailingAnchor.ConstraintEqualTo(TrailingAnchor).Active = true;
                //_scanView.TopAnchor.ConstraintEqualTo(SafeAreaLayoutGuide.TopAnchor).Active = true;
                //_scanView.BottomAnchor.ConstraintEqualTo(SafeAreaLayoutGuide.BottomAnchor).Active = true;

                Add(_scanView);

                _scanView.StartCamera();
                Task.Delay(500).ContinueWith((r) =>
                {
                    BeginInvokeOnMainThread(() => StartAnyline());
                });

                initialized = true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("EXCEPTION: " + e.Message);
                //ShowAlert("Init Error", e.Message);
            }
        }

        public override void SubviewAdded(UIView uiview)
        {
            base.SubviewAdded(uiview);
        }

        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);
        }

        public override void MovedToWindow()
        {
            base.MovedToWindow();
        }

        public override void WillMoveToWindow(UIWindow window)
        {
            base.WillMoveToWindow(window);
            if (window != null)
                InitializeAnyline(window);
            else
                Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _scanView.ScanViewPlugin.StopAndReturnError(out NSError error);

            if (error != null)
            {
                System.Diagnostics.Debug.WriteLine(error.DebugDescription);
            }
            _scanView?.Dispose();
            _scanView?.RemoveFromSuperview();
            _scanView = null;
        }

        private void StartAnyline()
        {
            NSError error = null;
            var success = _scanView.ScanViewPlugin.StartAndReturnError(out error);
            if (!success)
            {
                if (error != null)
                {
                    System.Diagnostics.Debug.WriteLine(error.DebugDescription);
                    //ShowAlert("Start Scanning Error", error.DebugDescription);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("error is null");
                    //ShowAlert("Start Scanning Error", "error is null");
                }
            }
        }

        private void ConnectDelegateToScanPlugin()
        {
            (_scanView.ScanViewPlugin as ALIDScanViewPlugin)?.IdScanPlugin.AddDelegate(_resultDelegate);
            (_scanView.ScanViewPlugin as ALBarcodeScanViewPlugin)?.BarcodeScanPlugin.AddDelegate(_resultDelegate);
            (_scanView.ScanViewPlugin as ALOCRScanViewPlugin)?.OcrScanPlugin.AddDelegate(_resultDelegate);
            (_scanView.ScanViewPlugin as ALMeterScanViewPlugin)?.MeterScanPlugin.AddDelegate(_resultDelegate);
            (_scanView.ScanViewPlugin as ALDocumentScanViewPlugin)?.DocumentScanPlugin.AddDelegate(_resultDelegate);
            (_scanView.ScanViewPlugin as ALLicensePlateScanViewPlugin)?.LicensePlateScanPlugin.AddDelegate(_resultDelegate);
            (_scanView.ScanViewPlugin as ALAbstractScanViewPluginComposite)?.AddDelegate(_resultDelegate);
        }
    }

    public class MyInfoDelegate : IALInfoDelegate
    {
        public override void ReportInfo(ALAbstractScanPlugin anylineScanPlugin, ALScanInfo info)
        {
            System.Diagnostics.Debug.WriteLine("INFO: " + info.Value.ToString());
        }

        public override void RunSkipped(ALAbstractScanPlugin anylineScanPlugin, ALRunSkippedReason runSkippedReason)
        {
            System.Diagnostics.Debug.WriteLine("INFO SKIPPED: " + runSkippedReason.Reason.ToString());
        }
    }
}
