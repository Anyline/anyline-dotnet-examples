using System;
using Anyline.Examples.MAUI.NFC;
using Anyline.Examples.MAUI.Platforms.iOS;
using Anyline.Examples.MAUI.Platforms.iOS.NFC;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform;
using CoreGraphics;
using Foundation;
using UIKit;
using Anyline.SDK.NET.iOS;

namespace Anyline.Examples.MAUI.Platforms.iOS.NFC
{
    public class NFCScanningViewRenderer : ViewRenderer, IALScanPluginDelegate, IALNFCDetectorDelegate
    {
        private CGRect frame;
        private bool initialized;

        private ALScanView _scanView;

        ALNFCDetector _nfcDetector;

        public override void WillMoveToWindow(UIWindow window)
        {
            base.WillMoveToWindow(window);
            if (window != null)
            {
                InitializeAnylineScanView();
                _nfcDetector = new ALNFCDetector(this, out NSError error);
            }
            else
            {
                DisposeAnyline();
            }
        }

        private void InitializeAnylineScanView()
        {
            try
            {
                NSError error = null;

                string configurationFile = (Element as NFCScanExampleView).JSONConfigPath.Replace(".json", "");

                // Use the JSON file name that you want to load here
                var configPath = NSBundle.MainBundle.PathForResource(configurationFile, @"json");
                // This is the main intialization method that will create our use case depending on the JSON configuration.
                _scanView = ALScanViewFactory.WithConfigFilePath(configPath, this, out error);

                if (error != null)
                {
                    throw new Exception(error.LocalizedDescription);
                }

                Add(_scanView);

                _scanView.StartCamera();
            }
            catch (Exception e)
            {
                ShowAlert("Init Error", e.Message);
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
            BeginInvokeOnMainThread(() => StartMRZScanner());
        }

        private void StartMRZScanner()
        {
            NSError error = null;
            var success = _scanView.ScanViewPlugin.StartWithError(out error);
            if (!success)
            {
                if (error != null)
                    ShowAlert("Start Scanning Error", error.DebugDescription);
                else
                    ShowAlert("Start Scanning Error", "error is null");
            }
        }

        private void ShowAlert(string title, string text)
        {
            new UIAlertView(title, text, (IUIAlertViewDelegate)null, "OK", null).Show();
        }

        [Export("scanPlugin:resultReceived:")]
        public void ResultReceived(ALScanPlugin scanPlugin, ALScanResult scanResult)
        {
            ALMrzResult mrzResult = scanResult.PluginResult.MrzResult;

            // Sends the MRZ results back to the Xamarin Forms application
            MyMRZScanResults myMRZScanResults = new MyMRZScanResults
            {
                GivenNames = mrzResult.GivenNames,
                Surname = mrzResult.Surname,
                CroppedImage = scanResult.CroppedImage.AsJPEG().ToArray(),
                FullImage = scanResult.FullSizeImage.AsJPEG().ToArray(),
                FaceImage = scanResult.FaceImage.AsJPEG().ToArray(),
                PassportNumber = mrzResult.DocumentNumber.Trim(),
                DateOfBirth = mrzResult.DateOfBirth,
                DateOfExpiry = mrzResult.DateOfExpiry
            };
            MessagingCenter.Send(App.Current, "MRZ_READING_DONE", myMRZScanResults);

            // Gets the data necessary for the NFC reading
            string passportNumber = mrzResult.DocumentNumber.Trim();
            NSDate dateOfBirth = ConvertToNSDate(mrzResult.DateOfBirthObject);
            NSDate dateOfExpiry = ConvertToNSDate(mrzResult.DateOfExpiryObject);

            // The passport number passed to the NFC chip must have a trailing < if there is one in the MRZ string.
            while (passportNumber.Length < 9)
            {
                passportNumber += "<";
            }

            // This is where we start reading the NFC chip of the passport.
            // We use data from the MRZ to authenticate with the chip.
            BeginInvokeOnMainThread(() =>
            {
                _nfcDetector.StartNfcDetectionWithPassportNumber(passportNumber, dateOfBirth, dateOfExpiry);
            });
        }

        /// <summary>
        /// Converts date strings from this: "Sun Apr 12 00:00:00 UTC 1977" to this: "04/12/1977"
        /// </summary>
        /// <param name="dateString">Date string to be converted</param>
        /// <returns>NSDate of the informed date</returns>
        private NSDate ConvertToNSDate(string dateString)
        {
            NSDateFormatter dateFormatter = new NSDateFormatter();
            dateFormatter.TimeZone = NSTimeZone.FromAbbreviation("GMT+0:00");
            dateFormatter.DateFormat = @"E MMM d HH:mm:ss zzz yyyy";
            dateFormatter.Locale = NSLocale.FromLocaleIdentifier("en_US_POSIX");
            NSDate nsDate = dateFormatter.Parse(dateString);
            return nsDate;
        }

        #region NFC Result
        public void NfcSucceededWithResult(ALNFCResult nfcResult)
        {
            MyNFCScanResults myNFCScanResults = new MyNFCScanResults
            {
                FirstName = nfcResult.DataGroup1.FirstName,
                LastName = nfcResult.DataGroup1.LastName, // The Last Name is returned without the spaces between the names (eg. SURNAME1SURNAME2)
                Gender = nfcResult.DataGroup1.Gender,
                DocumentNumber = nfcResult.DataGroup1.DocumentNumber,
                DateOfBirth = nfcResult.DataGroup1.DateOfBirth.ToString(),
                DateOfExpiry = nfcResult.DataGroup1.DateOfExpiry.ToString(),
                DocumentType = nfcResult.DataGroup1.DocumentType,
                IssuingStateCode = nfcResult.DataGroup1.IssuingStateCode,
                Nationality = nfcResult.DataGroup1.Nationality,
                FaceImage = nfcResult.DataGroup2.FaceImage.AsJPEG().ToArray()
            };
            
            // Sends the parsed results to the Message Listener (in this example, the NFCScanExamplePage)
            MessagingCenter.Send(App.Current, "NFC_SCAN_FINISHED_SUCCESS", myNFCScanResults);
        }

        public void NfcFailedWithError(NSError error)
        {
            MessagingCenter.Send(App.Current, "NFC_SCAN_FINISHED_ERROR", error.ToString());
            StartMRZScanner();
        }
        #endregion

        #region teardown
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
        #endregion
    }
}