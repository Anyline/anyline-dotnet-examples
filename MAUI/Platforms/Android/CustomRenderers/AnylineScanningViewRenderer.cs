using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using Anyline.Examples.MAUI.Views;
using IO.Anyline2;
using IO.Anyline2.View;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using IO.Anyline2.Viewplugin.AR.UiFeedback;

namespace Anyline.Examples.MAUI.Platforms.Android.CustomRenderers
{
    /// <summary>
    /// This class is responsible for rendering the Anyline ScanView natively.
    /// </summary>
    internal class AnylineScanningViewRenderer(Context context)
        : ViewRenderer(context), IEvent
    {
        private bool _initialized;
        private ScanView _scanView;
        private readonly Context _context = context;

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            (Element as AnylineScanningView).OnDisappearing = DisposeAnyline;
            InitializeAnyline();
        }

        private void InitializeAnyline()
        {
            if (_initialized)
                return;

            _scanView = new ScanView(_context);
            AddView(_scanView, new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent));
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
                    var scanningView = element as AnylineScanningView;
                    // Obtain the JSON config file path from the "AnylineScanningView", defined in the MAUI level.
                    string jsonConfigFilePath = scanningView.ScanMode.JSONConfigPath.Replace(".json", "") + ".json";

                    // This is the main intialization method that will create our use case depending on the JSON configuration.
                    try
                    {
                        scanView.Init(jsonConfigFilePath);
                        scanView.ScanViewPlugin.ResultReceived = parent;
                        scanView.ScanViewPlugin.ResultsReceived = parent;
                        scanView.ScanViewPlugin.UiFeedbackInfoReceived = new UIFeedbackLogger();
                        scanView.Start();

                        if (scanningView.ScanMode.ApplyBarcodeOverlays)
                        {
                            scanView.ScanViewPlugin.ActiveScanViewPlugin.First().EnableBarcodeOverlays(scanningView.BarcodeOverlayListener);
                        }
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
                var scanningView = Element as AnylineScanningView;

                Anyline.SDK.NET.Common.ScanResult[] scanResults = null;
                if (data is IO.Anyline2.ScanResult javaScanResult)
                {
                    scanResults = [javaScanResult.ToCommon()];
                }
                else if (data is JavaList javaScanResults)
                {
                    scanResults = new Anyline.SDK.NET.Common.ScanResult[javaScanResults.Size()];
                    for (var i = 0; i < javaScanResults.Size(); i++)
                    {
                        scanResults[i] = (javaScanResults.Get(i) as IO.Anyline2.ScanResult).ToCommon();
                    }
                }

                if (!scanningView.ScanMode.IsContinuous())
                {
                    // ensure lazy image memory streams are loaded before the javaScanResult is disposed
                    foreach (var scanResult in scanResults)
                    {
                        var imageMemoryStream = scanResult.ImageMemoryStream.Value;
                        var cutoutImageMemoryStream = scanResult.CutoutImageMemoryStream.Value;
                        var faceImageMemoryStream = scanResult.FaceImageMemoryStream.Value;
                    }
                }

                scanningView.OnResult?.Invoke(scanResults);
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