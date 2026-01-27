// TEMPLATE FILE - Copy to LicenseKey.cs and add your license key
//
// To use the Anyline SDK Example App:
// 1. Copy this file to LicenseKey.cs (in the same directory)
// 2. Replace "YOUR_LICENSE_KEY_HERE" with your actual Anyline license key
// 3. Build and run the app
//
// To get a license key:
// - Visit https://anyline.com to sign up for a trial or purchase a license
// - The license key should be base64 encoded
//
// Alternative: Use the generate script with environment variable:
//   export ANYLINE_MOBILE_SDK_LICENSE_KEY="your-license-key"
//   ./scripts/generate_license_key.sh

namespace Anyline.Examples.MAUI;

/// <summary>
/// Contains the Anyline SDK license key.
/// Replace the placeholder with your actual license key.
/// </summary>
public static class LicenseKey
{
    /// <summary>
    /// The Anyline SDK license key (base64 encoded).
    /// </summary>
    public static readonly string Value = "YOUR_LICENSE_KEY_HERE";
}
