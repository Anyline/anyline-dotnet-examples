using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Anyline.SDK.NET.Android;

namespace Anyline.Examples.MAUI;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    public static Activity Instance;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        Instance = this;

        //string myLicenseKey = "ewogICJsaWNlbnNlS2V5VmVyc2lvbiI6ICIzLjAiLAogICJkZWJ1Z1JlcG9ydGluZyI6ICJwaW5nIiwKICAibWFqb3JWZXJzaW9uIjogIjM3IiwKICAic2NvcGUiOiBbCiAgICAiQUxMIgogIF0sCiAgIm1heERheXNOb3RSZXBvcnRlZCI6IDUsCiAgImFkdmFuY2VkQmFyY29kZSI6IHRydWUsCiAgIm11bHRpQmFyY29kZSI6IHRydWUsCiAgInN1cHBvcnRlZEJhcmNvZGVGb3JtYXRzIjogWwogICAgIkFMTCIKICBdLAogICJwbGF0Zm9ybSI6IFsKICAgICJpT1MiLAogICAgIkFuZHJvaWQiCiAgXSwKICAic2hvd1dhdGVybWFyayI6IHRydWUsCiAgInRvbGVyYW5jZURheXMiOiAzMCwKICAidmFsaWQiOiAiMjAyMi0xMi0zMSIsCiAgImlvc0lkZW50aWZpZXIiOiBbCiAgICAiY29tLmFueWxpbmUueGFtYXJpbi5leGFtcGxlcyIsCiAgICAiY29tLmFueWxpbmUueGFtYXJpbi5mb3Jtcy5leGFtcGxlcyIsCiAgICAiY29tLmFueWxpbmUuZXhhbXBsZXMiCiAgXSwKICAiYW5kcm9pZElkZW50aWZpZXIiOiBbCiAgICAiY29tLmFueWxpbmUueGFtYXJpbi5leGFtcGxlcyIsCiAgICAiY29tLmFueWxpbmUueGFtYXJpbi5mb3Jtcy5leGFtcGxlcyIsCiAgICAiY29tLmFueWxpbmUuZXhhbXBsZXMiCiAgXQp9CnRad29IWnlXZmtYV1FldkRBUWdiNUYzQm1xVU9mOWQ2a3Vma0tsY1k0OU1CQWkybXZNUGI3N3JaRkhCeEJ1YUZjTmNrckJXbm83Yjl2U2RWWGNpdlQxcUx0MGtGK1BTMDlBb014alBCWjM3TllnQU5FTCtsdWF6UmhjVWJscmE2ek52UnpCdGhyblpPMy85WmVhZ0JYdTNCWFF3b0Vrc3p3TzJFVndTY0krNEdrb1hNTjFFU2ExL0YyNUhmMlBSay8yUmpvam9YeGdwR0hQVnJXcjRwUG03WlI4ZW1rUUtRU3N1U3NXTjdpMldsUFd0ekNuOU5HcjhvMWxxOUpEU1BvY3NmTXRqc2xwNjliM3Bibk9VR0k5dnFUUkdQZ0hZSktUSGVDVFZVdzlTWkZCb3psTFN4ZEJnalZWSUk1QW1xTTZRMjV1TVpvU044N3NWanhTaTE4Zz09";

        //try
        //{
        //    Anyline.SDK.NET.Android.AnylineSDK.Init(myLicenseKey, this);
        //}
        //catch (Exception ex)
        //{
        //    // Anyline SDK not initilized,
        //    // treat the exception accordingly.
        //    throw ex;
        //}
    }
}
