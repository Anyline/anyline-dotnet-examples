using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using Anyline.Examples.MAUI.Views;
using IO.Anyline.Camera;
using IO.Anyline.View;
using IO.Anyline2;
using IO.Anyline2.View;
using Java.Util;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform;

namespace Anyline.Examples.MAUI.Platforms.Android.CustomRenderers
{
    /// <summary>
    /// This class is responsible for rendering the Anyline ScanView natively.
    /// </summary>
    internal class AnylineScanningViewRenderer : ViewRenderer, IEvent
    {
        private bool _initialized;
        private IO.Anyline2.View.ScanView _scanView;
        private Context _context;

        public AnylineScanningViewRenderer(Context context) : base(context)
        {
            _context = context;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || Element == null)
            {
                return;
            }
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            InitializeAnyline();
        }

        private void InitializeAnyline()
        {
            if (_initialized)
                return;

            try
            {
                _scanView = new IO.Anyline2.View.ScanView(_context);

                // Obtain the JSON config file path from the "AnylineScanningView", defined in the MAUI level.
                string jsonConfigFilePath = (Element as AnylineScanningView).JSONConfigPath.Replace(".json", "") + ".json";

                // This is the main intialization method that will create our use case depending on the JSON configuration.
                _scanView.Init(jsonConfigFilePath);

                _scanView.ScanViewPlugin.ResultReceived = this;
                _scanView.ScanViewPlugin.ResultsReceived = this;

                // Handle camera open events
                _scanView.CameraView.CameraOpened += _scanView_CameraOpened;

                // Handle camera error events
                _scanView.CameraView.CameraError += _scanView_CameraError;

                _initialized = true;
                AddView(_scanView);
            }
            catch (Exception e)
            {
                // show error
                Toast.MakeText(_context, e.ToString(), ToastLength.Long).Show();
                Log.Debug("AnylineScanningViewRenderer - Android", e.ToString());
            }
        }

        private void _scanView_CameraError(object sender, CameraErrorEventArgs e)
        {
            Log.Debug("AnylineScanningViewRenderer - Android", e.ToString());
        }

        private void _scanView_CameraOpened(object sender, CameraOpenedEventArgs e)
        {
            if (_scanView != null)
                _scanView.Start();
        }

        /// <summary>
        /// This method is called when a scan result is found.
        /// Since the native Java type is generic, we translated the type of the parameter to ScanResult due to Xamarin.Android generic binding limitations.
        /// </summary>
        /// <param name="data">The scan result</param>
        public void EventReceived(Java.Lang.Object data)
        {
            if (data != null)
            {
                // Parse the result
                var dict = data.CreatePropertyDictionary();
                (Element as AnylineScanningView).OnResult?.Invoke(dict);
            }
        }

        /// <summary>
        /// On device rotated, dispose and re-initialize the ScanView.
        /// </summary>
        /// <param name="newConfig"></param>
        protected override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            DisposeAnyline();
            InitializeAnyline();
        }

        #region Teardown
        protected override void OnDetachedFromWindow()
        {
            base.OnDetachedFromWindow();
            DisposeAnyline();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            DisposeAnyline();
        }

        private void DisposeAnyline()
        {
            if (_scanView != null)
            {
                _scanView.Stop();
                _scanView.CameraView.CameraOpened -= _scanView_CameraOpened;
                _scanView.CameraView.CameraError -= _scanView_CameraError;
                _scanView.CameraView.ReleaseCameraInBackground();
                _scanView.Dispose();
                _scanView = null;
            }
            _initialized = false;
            RemoveAllViews();
        }
        #endregion

    }
}