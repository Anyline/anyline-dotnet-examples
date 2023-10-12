using Anyline.Examples.MAUI.Views;
using Anyline.SDK.NET.iOS;
using CoreGraphics;
using Foundation;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform;
using UIKit;

namespace Anyline.Examples.MAUI.Platforms.iOS.CustomRenderers
{
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
                InitializeAnylineScanView();
            else
                DisposeAnyline();
        }

        private void InitializeAnylineScanView()
        {
            NSError error = null;

            try
            {
                // Obtain the JSON config file path, defined in the MAUI level.
                string jsonConfigFilePath = (Element as AnylineScanningView).JSONConfigPath.Replace(".json", "");

                var configPath = NSBundle.MainBundle.PathForResource(jsonConfigFilePath, @"json");

                _resultDelegate = new ScanResultDelegate((Element as AnylineScanningView).OnResult);

                // This is the main intialization method that will create our use case depending on the JSON configuration.
                _scanView = ALScanViewFactory.WithConfigFilePath(configPath, _resultDelegate, out error);

                if (error != null)
                {
                    throw new Exception(error.LocalizedDescription);
                }

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
            
            if(_scanView == null){
                System.Diagnostics.Debug.WriteLine("ScanView is null and could not be started");
                return;
            }
            
            var success = _scanView.ScanViewPlugin.StartWithError(out error);
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
                _scanView.ScanViewPlugin.Stop();
            }

            _scanView?.Dispose();
            _scanView?.RemoveFromSuperview();
            _scanView = null;
        }
    }
}
