
using Foundation;

namespace Anyline
{
    public partial class AnylineSDKService
    {
        public partial bool SetupWithLicenseKey(string licenseKey, out string licenseErrorMessage)
        {
            Anyline.SDK.NET.iOS.AnylineSDK.SetupWithLicenseKey(licenseKey, out NSError licenseError);

            if (licenseError == null)
            {
                licenseErrorMessage = null;
                return true;
            }
            else
            {
                licenseErrorMessage = licenseError.LocalizedDescription;
                return false;
            }
        }
    }
}
