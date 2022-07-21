using Anyline.Examples.MAUI.Models;

namespace Anyline.Examples.MAUI
{
    /// <summary>
    /// 
    /// Only in use on iOS.
    /// This is a deprecated way of rendering the ScanView. 
    /// 
    /// </summary>
    internal class AnylineScanPage : ContentPage
    {
        public Action<object> OnResult;
        public string JSONConfigPath;

        /// <summary>
        /// This ContentPage is rendered natively in the iOS Plaform, and handles the complete scanning process. 
        /// The scanning results are received in the 'OnResult' action.
        /// </summary>
        /// <param name="scanMode">Object containing the Name of the ScanMode and the JSON config file path (used for initializing the ScanView).</param>
        public AnylineScanPage(AnylineScanMode scanMode)
        {
            BackgroundColor = Colors.Black;
            Title = scanMode.Name;
            JSONConfigPath = scanMode.JSONConfigPath;

            OnResult = (r) =>
            {
                var results = r as Dictionary<string, object>;
                DoSomethingWithResult(results, scanMode);
            };
        }

        private void DoSomethingWithResult(Dictionary<string, object> results, AnylineScanMode scanMode)
        {
            MainThread.BeginInvokeOnMainThread(new Action(async () =>
            {
                Navigation.InsertPageBefore(new ResultsPage(results, scanMode), Navigation.NavigationStack.Last());
                await Navigation.PopAsync();
            }));
        }
    }
}
