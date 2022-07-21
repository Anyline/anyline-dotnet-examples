using IO.Anyline.Models;
using IO.Anyline.Plugin;

namespace Anyline.Examples.MAUI.Platforms.Android
{
    // Currently not in use
    internal class MyScanResultListener : Java.Lang.Object, IScanResultListener
    {
        public void OnResult(Java.Lang.Object result)
        {
            var anylineResult = result as AnylineScanResult;
            System.Diagnostics.Debug.WriteLine(anylineResult.Result);
        }
    }
}
