namespace Anyline
{
    /// <summary>
    /// This class is responsible for the Initialization of the Anyline SDK.
    /// </summary>
    public partial class AnylineSDKService
    {
        /// <summary>
        /// This method is implemented inside the Android and iOS platforms via partial classes: AnylineSDKService.Android and AnylineSDKService.iOS.
        /// </summary>
        /// <param name="licenseKey">Your Anyline License Key created for your Bundle IDs.</param>
        /// <param name="licenseErrorMessage">Contains the error message when the SDK initialization fails.</param>
        /// <returns>True when the Anyline SDK is successfully initialized, False otherwise.</returns>
        public partial bool SetupWithLicenseKey(string licenseKey, out string licenseErrorMessage);
    }
}
