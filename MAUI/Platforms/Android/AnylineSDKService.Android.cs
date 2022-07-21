
namespace Anyline
{
    public partial class AnylineSDKService
    {
        public partial bool SetupWithLicenseKey(string licenseKey, out string licenseErrorMessage)
        {
            try
            {
                IO.Anyline.AnylineSDK.Init(licenseKey, context: Examples.MAUI.MainActivity.Instance);
                licenseErrorMessage = null;
                return true;
            }
            catch (Exception ex)
            {
                licenseErrorMessage = ex.Message;
                return false;
            }
        }
    }
}
