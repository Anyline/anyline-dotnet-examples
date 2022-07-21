using Anyline.SDK.NET.iOS;
using Foundation;

namespace Anyline.Examples.MAUI.Platforms.iOS
{
    /// <summary>
    /// This is the delegate class that implements all result callbacks for various ScanPlugins
    /// </summary>
    public sealed class ScanResultDelegate : NSObject,
        IALIDPluginDelegate,
        IALOCRScanPluginDelegate,
        IALMeterScanPluginDelegate,
        IALLicensePlateScanPluginDelegate,
        IALBarcodeScanPluginDelegate,
        IALTireScanPluginDelegate,
        IALCompositeScanPluginDelegate
    {
        private Action<object> resultsAction;

        public ScanResultDelegate(Action<object> resultsAction)
        {
            this.resultsAction = resultsAction;
        }

        // we call this method in every case a result is received and deal with processing that result in the ResultViewController
        void HandleResult(object result)
        {
            resultsAction?.Invoke(result.CreatePropertyDictionary());
        }

        public void DidFindResult(ALIDScanPlugin anylineIDScanPlugin, ALIDResult result)
        {
            HandleResult(result);
        }

        public void DidFindResult(ALOCRScanPlugin anylineOCRScanPlugin, ALOCRResult result)
        {
            System.Diagnostics.Debug.WriteLine(result.Result);
            HandleResult(result);
        }

        public void DidFindResult(ALMeterScanPlugin anylineMeterScanPlugin, ALMeterResult result)
        {
            HandleResult(result);
        }

        public void DidFindResult(ALLicensePlateScanPlugin anylineLicensePlateScanPlugin, ALLicensePlateResult result)
        {
            HandleResult(result);
        }

        public void DidFindResult(ALBarcodeScanPlugin anylineBarcodeScanPlugin, ALBarcodeResult result)
        {
            HandleResult(result);
        }

        public void DidFindResult(ALAbstractScanViewPluginComposite anylineCompositeScanPlugin, ALCompositeResult result)
        {
            HandleResult(result);
        }

        public void DidFindResult(ALTireScanPlugin anylineTireScanPlugin, ALTireResult result)
        {
            HandleResult(result);
        }
    }
}