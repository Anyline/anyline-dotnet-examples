using Anyline.SDK.NET.iOS;
using Foundation;

namespace Anyline.Examples.MAUI.Platforms.iOS
{
    /// <summary>
    /// This is the delegate class that implements the ResulReceived callback for the ScanPlugin
    /// </summary>
    public sealed class ScanResultDelegate : ALScanPluginDelegate, IALViewPluginCompositeDelegate
    {
        private Action<object> _resultsAction;

        public ScanResultDelegate(Action<object> resultsAction)
        {
            _resultsAction = resultsAction;
        }

        public override void ResultReceived(ALScanPlugin scanPlugin, ALScanResult scanResult)
        {
            _resultsAction?.Invoke(new[] { scanResult.ToCommon() });
        }

        [Export("viewPluginComposite:allResultsReceived:")]
        public void AllResultsReceived(ALViewPluginComposite viewPluginComposite, ALScanResult[] scanResults)
        {
            SDK.NET.Common.ScanResult[] results = new SDK.NET.Common.ScanResult[scanResults.Length];
            for (var i = 0; i < scanResults.Length; i++)
            {
                results[i] = scanResults[i].ToCommon();    
            }
           
            _resultsAction?.Invoke(results);
        }
    }
}