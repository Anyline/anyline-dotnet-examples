## Anyline .NET SDK ##

Anyline provides an easy-to-use SDK for applications to enable Optical Character Recognition (OCR) on mobile devices.

* The Anyline .NET SDK is essentially a Binding Library that wraps around the native Anyline SDK so all native functionality can be simply invoked via C# calls. 

* For more information, please refer to the documentation page of the Anyline [.NET](https://documentation.anyline.com/toc/platforms/dotnet) SDK, as well as the native Anyline [Android](https://documentation.anyline.com/toc/platforms/android/index.html) and [iOS](https://documentation.anyline.com/toc/platforms/ios/index.html) documentation pages.

## Quick Start ##

* Clone or Download this repository and open the Solution located inside the Examples/MAUI folder in Visual Studio.

## File summary ##

* `MAUI` - [MAUI](MAUI) example apps.
* `README.md` - This readme.
* `LICENSE.md` - The license file.

### Examples ###

The [Examples](Examples) directory provides source code for the Anyline MAUI Example apps. Simply build & run the example project for your desired platform and enjoy scanning :)
The examples app is designed in a generic way to work with any technical capability. In your specific implementation, it is advised to used the 

### Quick start & setup ###

For a detailed setup guide on how to integrate Anyline for your scanning application, please visit the [Anyline .NET documentation](https://documentation.anyline.io/toc/platforms/dotnet/index.html).


### Available Technical Capabilities ###
- [**Barcode:**](https://documentation.anyline.com/toc/products/barcode/index.html)  Scan 40+ types of international barcode & QR code formats.
- [**Meter:**](https://documentation.anyline.com/toc/products/meter/index.html) Scan meter readings of various electric, gas, and water meters.
- [**ID:**](https://documentation.anyline.com/toc/products/id/index.html) Reliable scanning of data from passports, IDs, Driver's Licenses, and  IDs' machine readable zones (MRZ).
- [**OCR:**](https://documentation.anyline.com/toc/products/anyline_ocr/index.html) Create your own custom use case.
- [**License Plate:**](https://documentation.anyline.com/toc/products/license_plate/index.html) Scan license plates.
- [**Tire:**](https://documentation.anyline.com/toc/products/tire/index.html) Scan tire side-wall information such as *identification number* and *size*.

### Requirements ###

- Visual Studio with the ".NET Multi-platform App UI development" (MAUI) package installed.


#### Android ####

- Android device with SDK >= 24 (Android 7.0).
- Decent camera functionality (recommended: 720p and adequate auto focus).


#### iOS ####

- Minimum iOS 12.0.
- Minimum iPhone 5s.
- A Mac computer for building / deploying to the iPhone.


## Get Help (Support) ##

We don't actively monitor the GitHub Issues, please raise a support request using the [Anyline Helpdesk](https://anyline.atlassian.net/servicedesk/customer/portal/2/group/6).
When raising a support request based on this GitHub Issue, please fill out and include the following information:

```
Support request concerning Anyline GitHub Repository: anyline-ocr-dotnet-module
```

Thank you!


## License ##

See LICENSE file.