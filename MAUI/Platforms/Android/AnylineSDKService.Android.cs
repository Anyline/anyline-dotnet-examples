
namespace Anyline
{
    public partial class AnylineSDKService
    {
        public partial bool SetupWithLicenseKey(string licenseKey, out string licenseErrorMessage)
        {
            try
            {
                IO.Anyline2.AnylineSdk.Init(licenseKey, context: Examples.MAUI.MainActivity.Instance);
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
            return IO.Anyline2.AnylineSdk.GetPluginVersion().ToString();
        }

        public String GetSDKVersion()
        {
            return AT.Nineyards.Anyline.BuildConfig.VersionName;
        }
    }
}
