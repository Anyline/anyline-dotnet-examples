using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Widget;
using Anyline.Examples.MAUI.NFC;
using Anyline.Examples.MAUI.Platforms.Android.NFC;
using IO.Anyline2.Camera;
using IO.Anyline.Plugin.Result;
using IO.Anyline2;
using IO.Anyline2.View;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform;

namespace Anyline.Examples.MAUI.Platforms.Android.NFC
{
    public class NFCScanningViewRenderer : ViewRenderer, IEvent
    {
        private bool _initialized;
        private IO.Anyline2.View.ScanView _scanView;
        private Context _context;

        public NFCScanningViewRenderer(Context context) : base(context)
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
                string jsonConfigFilePath = (Element as NFCScanExampleView).JSONConfigPath.Replace(".json", "") + ".json";

                // This is the main intialization method that will create our use case depending on the JSON configuration.
                _scanView.Init(jsonConfigFilePath);

                _scanView.ScanViewPlugin.ResultReceived = this;

                // Handle camera open events
                _scanView.CameraView.CameraOpened += _scanView_CameraOpened;

                // Handle camera error events
                _scanView.CameraView.CameraError += _scanView_CameraError;

                AddView(_scanView);
            }
            catch (Exception e)
            {
                // show error
                Toast.MakeText(_context, e.ToString(), ToastLength.Long).Show();
                Log.Debug("NFCScanningViewRenderer - Android", e.ToString());
            }
        }

        private void _scanView_CameraError(object sender, CameraErrorEventArgs e)
        {
            Log.Debug("NFCScanningViewRenderer - Android", e.ToString());
        }

        private void _scanView_CameraOpened(object sender, CameraOpenedEventArgs e)
        {
            if (_scanView != null)
                _scanView.Start();
        }
        

        /// <summary>
        /// This method is called when a scan result is found.
        /// Since the native Java type is generic, we translated the type of the parameter to ScanResult due to .NET Android generic binding limitations.
        /// </summary>
        /// <param name="data">The scan result</param>
        public void EventReceived(Java.Lang.Object data)
        {
            if (data == null) return;

            var scanResult = data as IO.Anyline2.ScanResult;
            PluginResult pluginResult = scanResult.PluginResult;
            var mrzResult = pluginResult.MrzResult;

            // Sends the MRZ results back to the MAUI layer
            MyMRZScanResults myMRZScanResults = new MyMRZScanResults
            {
                GivenNames = mrzResult.GivenNames,
                Surname = mrzResult.Surname,
                CroppedImage = ConvertAnylineImageToByteArray(scanResult.CutoutImage),
                FullImage = ConvertAnylineImageToByteArray(scanResult.Image),
                //FaceImage = ConvertBitmapToByteArray(scanResult.FaceImage), //not available yet
                PassportNumber = mrzResult.DocumentNumber.Trim(),
                DateOfBirth = mrzResult.DateOfBirth,
                DateOfExpiry = mrzResult.DateOfExpiry
            };
            MessagingCenter.Send(App.Current, "MRZ_READING_DONE", myMRZScanResults);

            StartReadingNFCChip(mrzResult.DocumentNumber, mrzResult.DateOfBirth, mrzResult.DateOfExpiry);
        }

        private void StartReadingNFCChip(string documentNumber, string dateOfBirth, string dateOfExpiry)
        {
            // Gets the data necessary for the NFC reading
            string passportNumber = documentNumber.Trim();
            // The passport number passed to the NFC chip must have a trailing < if there is one in the MRZ string.
            while (passportNumber.Length < 9)
            {
                passportNumber += "<";
            }

            // Open the Activity responsible for listening to the NFC calls and reading the chip.
            // We use data from the MRZ to authenticate with the chip.

            var activity = this.Context as Activity;
            var nfcActivity = new Intent(activity, typeof(NFCScanActivity));
            nfcActivity.PutExtra("pn", passportNumber);
            nfcActivity.PutExtra("db", dateOfBirth);
            nfcActivity.PutExtra("de", dateOfExpiry);
            activity.StartActivityForResult(nfcActivity, 0);
        }

        private byte[] ConvertAnylineImageToByteArray(IO.Anyline2.Image.AnylineImage anylineImage)
        {
            if (anylineImage == null) return null;

            var bitmap = anylineImage.Bitmap;

            MemoryStream stream = new MemoryStream();
            bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
            return stream.ToArray();
        }

        private byte[] ConvertBitmapToByteArray(Bitmap bitmap)
        {
            if (bitmap == null) return null;

            MemoryStream stream = new MemoryStream();
            bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
            return stream.ToArray();
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