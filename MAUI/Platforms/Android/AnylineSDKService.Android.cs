using IO.Anyline2;
using IO.Anyline2.Init;


namespace Anyline
{
    public partial class AnylineSDKService
    {
        public partial bool SetupWithLicenseKey(string licenseKey, out string licenseErrorMessage)
        {
            try
            {
                AnylineSdk.Init(
                    context: Examples.MAUI.MainActivity.Instance, 
                    new SdkInitializationConfig(
                        new SdkInitializationParameters(licenseKey: licenseKey),
                        SdkInitializationStrategy.SyncManual.Instance)
                    );                
                licenseErrorMessage = null;
                return true;
            }
            catch (Exception ex)
            {
                licenseErrorMessage = ex.Message;
                return false;
            }
        }

        public String GetPluginVersion()
        {           
            return AnylineSdk.GetPluginVersion().ToString();
        }

        public String GetSDKVersion()
        {
            return AT.Nineyards.Anyline.BuildConfig.VersionName;
        }
    }
}
