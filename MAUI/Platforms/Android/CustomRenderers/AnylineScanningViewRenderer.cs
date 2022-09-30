using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Anyline.Examples.MAUI.Views;
using IO.Anyline.Camera;
using IO.Anyline.Models;
using IO.Anyline.Nfc.NFC;
using IO.Anyline.Plugin;
using IO.Anyline.Plugin.ID;
using IO.Anyline.View;
using Java.Util;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform;

namespace Anyline.Examples.MAUI.Platforms.Android.CustomRenderers
{
    /// <summary>
    /// This class is responsible for rendering the Anyline ScanView natively.
    /// </summary>
    internal class AnylineScanningViewRenderer : ViewRenderer, IScanResultListener
    {
        private bool initialized;
        ScanView _scanView;

        public AnylineScanningViewRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || Element == null)
            {
                return;
            }
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            InitializeAnyline();
        }

        private void InitializeAnyline()
        {
            if (initialized)
                return;

            try
            {
                var activity = Context as Activity;

                _scanView = new ScanView(Context);

                // Obtain the JSON config file path from the "AnylineScanningView", defined in the MAUI level.
                string jsonConfigFilePath = (Element as AnylineScanningView).JSONConfigPath.Replace(".json", "") + ".json";

                // This is the main intialization method that will create our use case depending on the JSON configuration.
                _scanView.Init(jsonConfigFilePath);

                // Activates Face Detection if the MRZ Scanner was initialized
                (((_scanView.ScanViewPlugin as IdScanViewPlugin)?.ScanPlugin as IdScanPlugin)?.IdConfig as MrzConfig)?.EnableFaceDetection(true);

                _scanView.ScanViewPlugin.AddScanResultListener(this);

                // Handle camera open events
                _scanView.CameraOpened += _scanView_CameraOpened;

                // Handle camera error events
                _scanView.CameraError += _scanView_CameraError;
            }
            catch (Exception e)
            {
                // show error
                Log.Debug("AnylineScanningViewRenderer - Android", e.ToString());
            }

            initialized = true;
            AddView(_scanView);
        }

        private void _scanView_CameraError(object sender, CameraErrorEventArgs e)
        {
            Log.Debug("AnylineScanningViewRenderer - Android", e.ToString());
        }

        private void _scanView_CameraOpened(object sender, CameraOpenedEventArgs e)
        {
            if (_scanView != null)
                _scanView.Start();
        }

        public void OnResult(Java.Lang.Object result)
        {
            // Parses the result
            AnylineScanResult anylineScanResult = result as AnylineScanResult;
            var dict = CreatePropertyList(anylineScanResult);
            // Sends the results back to the MAUI layer via "OnResult" action, defined in the AnylineScanningView.
            (Element as AnylineScanningView).OnResult?.Invoke(dict);
        }

        /// <summary>
        /// On device rotated, dispose and re-initialize the ScanView.
        /// </summary>
        /// <param name="newConfig"></param>
        protected override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            DisposeAnyline();
            InitializeAnyline();
        }

        protected override void OnDetachedFromWindow()
        {
            base.OnDetachedFromWindow();

            DisposeAnyline();
            RemoveAllViews();
            Dispose();
        }

        private void DisposeAnyline()
        {
            if (_scanView != null)
            {
                _scanView.Stop();
                _scanView.CameraView.ReleaseCameraInBackground();

                _scanView.CameraOpened -= _scanView_CameraOpened;
                _scanView.CameraError -= _scanView_CameraError;

                _scanView.ScanViewPlugin.RemoveScanResultListener(this);
                _scanView.ScanViewPlugin.Dispose();
                _scanView.CameraController.Dispose();

                _scanView.RemoveAllViews();
                _scanView.CameraView.Dispose();
                _scanView = null;
            }
            initialized = false;
        }

        /// <summary>
        /// This method is a generic way of display the properties and values for any technical capability.
        /// In your use-case, it is advisable to parse the AnylineScanResult directly into the specific plugin objects.
        /// Check the "PLUGIN SPECIFICS" section on https://documentation.anyline.com/toc/platforms/android/plugins/index.html for more information.
        /// </summary>
        /// <param name="obj">Generic result object</param>
        /// <returns>Dictionary containing all of the property/values of any scan result.</returns>
        private Dictionary<string, object> CreatePropertyList(Java.Lang.Object obj)
        {
            var dict = new Dictionary<string, object>();
            int resultGroupsIndex = 0;
            foreach (var prop in obj.GetType().GetProperties())
            {
                switch (prop.Name)
                {
                    // filter out properties that we don't want to display
                    case "JniPeerMembers":
                    case "JniIdentityHashCode":
                    case "Handle":
                    case "PeerReference":
                    case "Outline":
                    case "Class":
                    case "FieldNames":
                    case "Coordinates":
                        break;
                    default:

                        try
                        {
                            var value = prop.GetValue(obj, null);

                            // filter out deprecated fields
                            if (prop.GetCustomAttributes(typeof(ObsoleteAttribute), true).ToArray().Length > 0)
                                continue;

                            if (value != null)
                            {
                                // Iterate through a list of results
                                if (value is JavaList)
                                {
                                    var indexResult = 0;

                                    var mapResultGroup = new Dictionary<string, object>();
                                    foreach (Java.Lang.Object result in (value as JavaList))
                                    {
                                        var sublist = CreatePropertyList(result);
                                        var mapPluginResults = new Dictionary<string, object>();
                                        foreach (KeyValuePair<string, object> item in sublist)
                                        {
                                            mapPluginResults.Add(item.Key, item.Value);
                                        }
                                        mapResultGroup.Add(indexResult.ToString(), mapPluginResults);
                                        indexResult++;
                                    }
                                    dict.Add(prop.Name, mapResultGroup);
                                    resultGroupsIndex++;
                                }
                                else if (value is AnylineImage)
                                {
                                    var bitmap = (value as AnylineImage).Clone().Bitmap;

                                    MemoryStream stream = new MemoryStream();
                                    bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
                                    byte[] bitmapData = stream.ToArray();

                                    dict.Add(prop.Name, bitmapData);
                                }
                                else if (value is Bitmap bitmap)
                                {
                                    MemoryStream stream = new MemoryStream();
                                    bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
                                    byte[] bitmapData = stream.ToArray();

                                    dict.Add(prop.Name, bitmapData);
                                }
                                else if (value is ID id)
                                {
                                    var sublist = CreatePropertyList(id);
                                    sublist.ToList().ForEach(x => dict.Add(x.Key, x.Value));
                                }
                                else if (value is IDFieldConfidences idFieldConfidences)
                                {
                                    var sublist = CreatePropertyList(idFieldConfidences);
                                    sublist.ToList().ForEach(x => dict.Add($"{x.Key} (field confidence)", x.Value));
                                }
                                else if (value is LayoutDefinition universalIDInfo)
                                {
                                    var sublist = CreatePropertyList(universalIDInfo);
                                    sublist.ToList().ForEach(x => dict.Add($"{x.Key}", x.Value));
                                }
                                else if (value is JavaDictionary<String, String> dictionaryValues)
                                {
                                    foreach (var v in dictionaryValues)
                                        dict.Add(v.Key, new Java.Lang.String(v.Value.ToString()).ReplaceAll("\\\\n", "\\\n"));
                                }
                                else if (value is DataGroup1 dg1)
                                {
                                    var sublist = CreatePropertyList(dg1);
                                    sublist.ToList().ForEach(x => dict.Add($"{x.Key}", x.Value));
                                }
                                else if (value is DataGroup2 dg2)
                                {
                                    var sublist = CreatePropertyList(dg2);
                                    sublist.ToList().ForEach(x => dict.Add($"{x.Key}", x.Value));
                                }
                                else if (value is SOD sod)
                                {
                                    var sublist = CreatePropertyList(sod);
                                    sublist.ToList().ForEach(x => dict.Add($"{x.Key}", x.Value));
                                }
                                else if (value is IO.Anyline.Plugin.Barcode.PDF417 pdf417)
                                {
                                    var body = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(pdf417.Body);
                                    body.ToList().ForEach(x => dict.Add($"{x.Key} (parsed info)", x.Value));

                                    var additionalInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(pdf417.AdditionalInformation);
                                    additionalInfo.ToList().ForEach(x => dict.Add($"{x.Key} (parsed info)", x.Value));
                                }
                                else
                                {
                                    var str = new Java.Lang.String(value.ToString()).ReplaceAll("\\\\n", "\\\n");
                                    dict.Add(prop.Name, str);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine("Exception: " + e.Message);
                        }
                        break;
                }
            }

            // re-order the list so that the plugin ID and the result are presented first:
            MoveElementToIndex(dict, "PluginId", 0);
            MoveElementToIndex(dict, "Result", 1);

            return dict;
        }

        public static void MoveElementToIndex<T>(Dictionary<string, T> dict, string identifier, int pos)
        {
            var result = new Dictionary<string, T>();

            if (dict.ContainsKey(identifier))
            {
                var list = dict.ToList();

                var currentIndex = list.FindIndex(x => x.Key == identifier);
                var currentElement = list.ElementAt(currentIndex);

                list.Remove(currentElement);
                list.Insert(pos, currentElement);

                dict.Clear();
                list.ForEach(x => dict.Add(x.Key, x.Value));
            }
        }
    }
}