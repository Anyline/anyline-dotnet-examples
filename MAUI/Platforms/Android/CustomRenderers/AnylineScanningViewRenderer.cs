using Android.Content;
using Android.Content.Res;
using Android.Util;
using Android.Widget;
using Anyline.Examples.MAUI.Views;
using IO.Anyline2;
using IO.Anyline2.View;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform;
using IO.Anyline2.Viewplugin.AR.UiFeedback;

namespace Anyline.Examples.MAUI.Platforms.Android.CustomRenderers
{
    /// <summary>
    /// This class is responsible for rendering the Anyline ScanView natively.
    /// </summary>
    internal class AnylineScanningViewRenderer(Context context) : ViewRenderer(context), IEvent
    {
        private bool _initialized;
        private ScanView _scanView;
        private readonly Context _context = context;

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

            _scanView = new ScanView(_context);
            AddView(_scanView);                
            _scanView.SetOnScanViewLoaded(new ScanViewLoadHandler(this, Element, _scanView));
            _initialized = true;
        }

        class ScanViewLoadHandler(AnylineScanningViewRenderer parent, View element, ScanView scanView) : Java.Lang.Object, IEvent
        {
            void IEvent.EventReceived(Java.Lang.Object args)
            {
                var scanViewLoadResult = (ScanViewLoadResult) args;
                if (scanViewLoadResult is ScanViewLoadResult.Succeeded succeeded)
                {
                    // Obtain the JSON config file path from the "AnylineScanningView", defined in the MAUI level.
                    string jsonConfigFilePath = (element as AnylineScanningView).JSONConfigPath.Replace(".json", "") + ".json";

                    // This is the main intialization method that will create our use case depending on the JSON configuration.
                    try
                    {
                        scanView.Init(jsonConfigFilePath);
                        scanView.ScanViewPlugin.ResultReceived = parent;
                        scanView.ScanViewPlugin.ResultsReceived = parent;
                        scanView.ScanViewPlugin.UiFeedbackInfoReceived = new UIFeedbackLogger();                        
                        scanView.Start();    
                    } catch (Exception e)
                    {
                        // show error
                        Toast.MakeText(scanView.Context, e.ToString(), ToastLength.Long).Show();
                        Log.Debug("AnylineScanningViewRenderer - Android", e.ToString());
                    }
                }
                else if (scanViewLoadResult is ScanViewLoadResult.Failed { ErrorMessage: not null } failed)
                {
                    // show error
                    Toast.MakeText(scanView.Context, failed.ErrorMessage, ToastLength.Long).Show();
                    Log.Debug("AnylineScanningViewRenderer - Android", failed.ErrorMessage);
                }
            }
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

        class UIFeedbackLogger : Java.Lang.Object, IEvent
        {
            void IEvent.EventReceived(Java.Lang.Object data)
            {
                var json = (Org.Json.JSONObject)data;
                var messageArray = json.OptJSONArray("messages");
                if (messageArray != null)
                {
                    for (var i = 0; i < messageArray.Length(); i++)
                    {
                        var msgEntry = UIFeedbackOverlayInfoEntry.FromJson((Org.Json.JSONObject) messageArray.Get(i));
                        if (msgEntry.GetLevel() == UIFeedbackOverlayInfoEntry.Level.Info)
                        {
                            Log.Info("AnylineScanningViewRenderer - Android", "UIFeedbackInfo: " + msgEntry.Message);
                        }
                        else if (msgEntry.GetLevel() == UIFeedbackOverlayInfoEntry.Level.Warning)
                        {
                            Log.Warn("AnylineScanningViewRenderer - Android", "UIFeedbackWarn: " + msgEntry.Message);
                        }
                        else if (msgEntry.GetLevel() == UIFeedbackOverlayInfoEntry.Level.Error)
                        {
                            Log.Error("AnylineScanningViewRenderer - Android", "UIFeedbackError: " + msgEntry.Message);
                        }
                    }
                }               
            }
        }

        /// <summary>
        /// On layout change, propagate changes to ScanView.
        /// </summary>
        /// <param name="changed"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            base.OnLayout(changed, left, top, right, bottom);
            if (_scanView != null)
                _scanView.Layout(left, top, right, bottom);
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
            DisposeAnyline();
            base.OnDetachedFromWindow();
        }

        private void DisposeAnyline()
        {
            if (_scanView != null)
            {
                if (_scanView.IsInitialized)
                {
                    _scanView.Stop();
                }                    
                _scanView.Dispose();
                _scanView = null;
            }
            _initialized = false;
            RemoveAllViews();
        }
        #endregion

    }
}