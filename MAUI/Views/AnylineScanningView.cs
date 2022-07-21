
namespace Anyline.Examples.MAUI.Views
{
    /// <summary>
    /// 
    /// (Currently only in use on Android)
    /// 
    /// This ContentView is rendered natively in each individual platform. 
    /// The scanning results are received in the 'OnResult' action.
    /// </summary>
    internal class AnylineScanningView : ContentView
    {
        public string JSONConfigPath;
        public Action<object> OnResult;

        /// <summary>
        /// Holds the information necessary for the ScanView Initialization and Result processing.
        /// </summary>
        /// <param name="jsonConfigPath">The path to the JSON config file.</param>
        /// <param name="onResultAction">The Action that should be called once the results are available.</param>
        public AnylineScanningView(string jsonConfigPath, Action<object> onResultAction)
        {
            JSONConfigPath = jsonConfigPath;
            OnResult = onResultAction;
            BackgroundColor = Color.FromArgb("00000000");
        }
    }
}