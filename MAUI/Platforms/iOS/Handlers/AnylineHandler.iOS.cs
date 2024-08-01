using Anyline.Examples.MAUI.Handlers;
using Anyline.Examples.MAUI.Platforms.iOS;
using Anyline.SDK.NET.iOS;
using CoreGraphics;
using Foundation;
using MapKit;
using Microsoft.Maui.Handlers;
using SceneKit;
using UIKit;

namespace Anyline.Examples.MAUI.Handlers
{
    public partial class AnylineHandler : ViewHandler<AnylineView, UIView>
    {
        private ALScanView _scanView;
        ScanResultDelegate _resultDelegate;

        public AnylineHandler(IPropertyMapper mapper, CommandMapper commandMapper = null) : base(mapper, commandMapper)
        { }

        protected override UIView CreatePlatformView()
        {
            NSError error = null;
            //string configurationFile = (Element as MyScanningView).ConfigurationFile.Replace(".json", "");
            string configurationFile = "Configs/usnr_config";
            // Use the JSON file name that you want to load here
            var configPath = NSBundle.MainBundle.PathForResource(configurationFile, @"json");
            System.Diagnostics.Debug.WriteLine("Config PATH: " + configPath);
            // This is the main intialization method that will create our use case depending on the JSON configuration.
            _resultDelegate = new ScanResultDelegate((r) =>
            {
                System.Diagnostics.Debug.WriteLine(r.ToString());
            });

            //_scanView = ALScanView.ScanViewForFrame(VirtualView.Bounds, configPath, _resultDelegate, out error);

            //_scanView = ALScanView.ScanViewForFrame(new CGRect(0, 0, 320, 520), configPath, _resultDelegate, out error);
            _scanView = ALScanViewFactory.WithConfigFilePath(configPath, _resultDelegate, out error);
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


            _scanView.TranslatesAutoresizingMaskIntoConstraints = true;

            _scanView.StartCamera();

            return _scanView;
        }

        //view == AnylineView
        public override void SetVirtualView(IView view)
        {
            base.SetVirtualView(view);
        }

        //PlatformView == ALScanView
        protected override void ConnectHandler(UIView PlatformView)
        {
            // TODO: (RC) Remove this delay and use some "ViewDidAppear" kind of event
            // could not find any until now
            Task.Delay(1000).ContinueWith((r) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    //_scanView.LeadingAnchor.ConstraintEqualTo(ContainerView.LeadingAnchor).Active = true;
                    //_scanView.TrailingAnchor.ConstraintEqualTo(ContainerView.TrailingAnchor).Active = true;
                    //_scanView.TopAnchor.ConstraintEqualTo(ContainerView.SafeAreaLayoutGuide.TopAnchor).Active = true;
                    //_scanView.BottomAnchor.ConstraintEqualTo(ContainerView.SafeAreaLayoutGuide.BottomAnchor).Active = true;

                    StartAnyline();
                });
            });
        }

        private void StartAnyline()
        {
            NSError error = null;
            bool success = _scanView.ViewPlugin.StartWithError(out error);

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


        protected override void DisconnectHandler(UIView PlatformView)
        {
            //// Clean-up the native view to reduce memory leaks and memory usage
            //if (PlatformView.Delegate != null)
            //{
            //    PlatformView.Delegate.Dispose();
            //    PlatformView.Delegate = null;
            //}
            _scanView.StopCamera();
            _scanView.ViewPlugin.Stop();
            PlatformView.RemoveFromSuperview();
        }
    }
}
