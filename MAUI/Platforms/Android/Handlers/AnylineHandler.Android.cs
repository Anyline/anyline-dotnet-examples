using Android.App;
using Android.Util;
using Anyline.Examples.MAUI.Platforms.Android;
using IO.Anyline.Camera;
using IO.Anyline.Plugin.ID;
using IO.Anyline.View;
using Microsoft.Maui.Handlers;

namespace Anyline.Examples.MAUI.Handlers
{
    /* TODO: (RC) Issues:
     * - No access to the View Lifecycle, and the "Dispose" methods are not being called by default. No way do properly close the ScanView.
     * 
     */

    public partial class AnylineHandler : ViewHandler<AnylineView, Android.Views.View>
    {
        private ScanView _scanView;

        public AnylineHandler(IPropertyMapper mapper, CommandMapper commandMapper = null) : base(mapper, commandMapper)
        { }

        protected override void ConnectHandler(Android.Views.View platformView)
        {
            base.ConnectHandler(platformView);
        }

        protected override Android.Views.View CreatePlatformView()
        {
            try
            {
                var activity = Context as Activity;

                _scanView = new ScanView(Context);

                _scanView.Init("Configs/usnr_config.json");

                // Activates Face Detection if the MRZ Scanner was initialized
                (((_scanView.ScanViewPlugin as IdScanViewPlugin)?.ScanPlugin as IdScanPlugin)?.IdConfig as MrzConfig)?.EnableFaceDetection(true);

                _scanView.ScanViewPlugin.AddScanResultListener(new MyScanResultListener());

                // handle camera open events
                _scanView.CameraOpened += _scanView_CameraOpened;

                // handle camera error events
                _scanView.CameraError += _scanView_CameraError;
            }
            catch (Exception e)
            {
                // show error
                Log.Debug("AnylineHandler.Android", e.ToString());
            }
            return _scanView;
        }

        private void _scanView_CameraError(object sender, CameraErrorEventArgs e)
        {
            Log.Debug("AnylineHandler.Android", e.ToString());
        }

        private void _scanView_CameraOpened(object sender, CameraOpenedEventArgs e)
        {
            if (_scanView != null)
                _scanView.Start();
        }

        protected override void RemoveContainer()
        {
            base.RemoveContainer();
        }

        protected override void DisconnectHandler(Android.Views.View platformView)
        {
            base.DisconnectHandler(platformView);



            if (_scanView != null)
            {
                _scanView.Stop();
                _scanView.CameraView.ReleaseCamera();
                _scanView.Dispose();
                _scanView.CameraOpened -= _scanView_CameraOpened;
                _scanView.CameraError -= _scanView_CameraError;
                _scanView = null;
            }

            platformView.Dispose();
            platformView = null;
        }
    }
}
