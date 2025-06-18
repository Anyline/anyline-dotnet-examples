
using Anyline.SDK.NET.Common.Config;
using Anyline.SDK.NET.Common.ScanView.BarcodeOverlays;
using Microsoft.Maui.Platform;
using TextAlignment = Microsoft.Maui.TextAlignment;

namespace Anyline.Examples.MAUI.Models.BarcodeOverlays;

public class BarcodeOverlayViewList: List<CommonOverlayView>
{
    public readonly OverlayType OverlayType;

    private static Lazy<Color> _lazySelectedOverlayColor = new(() =>
    {
        if (Application.Current.Resources.TryGetValue("SelectedOverlay", out var colorObj) && colorObj is Color color)
        {
            return color;
        }
        return null;
    });
    
    public BarcodeOverlayViewList(CommonBarcodeOverlayListenerImpl commonBarcodeOverlayListener, CommonVisibleBarcode commonVisibleBarcode)
    {
        OverlayType = OverlayType.GetOverlayTypeFromVisibleBarcode(
            commonVisibleBarcode: commonVisibleBarcode, 
            isSelected: commonBarcodeOverlayListener.IsSelected(commonVisibleBarcode.Value));
        SetOverlayViewHolders(commonBarcodeOverlayListener, commonVisibleBarcode);
    }
    
    private void SetOverlayViewHolders(CommonBarcodeOverlayListenerImpl commonBarcodeOverlayListener, CommonVisibleBarcode mauiVisibleBarcode)
    {
        Clear();
        var imageButton = new ImageButton()
        {
            Aspect = Aspect.AspectFit,
            Source = OverlayType.GetImageSource(),
            WidthRequest = 50,
            HeightRequest = 50,
            BackgroundColor = Colors.Transparent
        };

        imageButton.Clicked += (s, e) =>
        {
            var barcodeValue = mauiVisibleBarcode.Value;
            commonBarcodeOverlayListener.SetSelected(!commonBarcodeOverlayListener.IsSelected(barcodeValue), mauiVisibleBarcode);
            mauiVisibleBarcode.Invalidate();
        };

        var imageButtonOverlayConfig = new OverlayConfig()
        {
            Anchor = OverlayAnchorConfig.Center,
            SizeDimension = new OverlayDimensionConfig()
            {
                ScaleX = new OverlayScaleConfig() {
                    ScaleValue = 0.7,
                    ScaleType = OverlayScaleTypeConfig.Overlay
                },
                ScaleY = new OverlayScaleConfig() {
                    ScaleValue = 0.7,
                    ScaleType = OverlayScaleTypeConfig.Overlay
                }
            }
        };
        
        var imageButtonViewHandler = GetPlatformViewHandlerFromVisualElement(imageButton);
        Add(new CommonOverlayView(imageButtonViewHandler, imageButtonViewHandler.PlatformView, imageButtonOverlayConfig));
        
        if (OverlayType is OverlayTypeSelected)
        {
            var label = new Label()
            {
                Text = mauiVisibleBarcode.Value,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                FontAttributes = FontAttributes.Bold,
                WidthRequest = 50,
                HeightRequest = 50,    
            };
            if (_lazySelectedOverlayColor.Value != null)
            {
                label.TextColor = Colors.Black;
                label.BackgroundColor = _lazySelectedOverlayColor.Value;
            }
            var labelOverlayConfig = new OverlayConfig()
            {
                Anchor = OverlayAnchorConfig.BottomCenter,
                SizeDimension = new OverlayDimensionConfig()
                {
                    ScaleX = new OverlayScaleConfig() {
                        ScaleValue = 1.0,
                        ScaleType = OverlayScaleTypeConfig.Overlay
                    },
                    ScaleY = new OverlayScaleConfig() {
                        ScaleValue = 0.3,
                        ScaleType = OverlayScaleTypeConfig.Overlay
                    }
                },
                OffsetDimension = new OverlayDimensionConfig()
                {
                    ScaleX = new OverlayScaleConfig() {
                        ScaleValue = 0,
                        ScaleType = OverlayScaleTypeConfig.FixedPx
                    },
                    ScaleY = new OverlayScaleConfig() {
                        ScaleValue = -0.1,
                        ScaleType = OverlayScaleTypeConfig.Overlay
                    }
                }
            };            
            
            var labelViewHandler = GetPlatformViewHandlerFromVisualElement(label);
            Add(new CommonOverlayView(labelViewHandler, labelViewHandler.PlatformView, labelOverlayConfig));
        }
    }    

    private static IPlatformViewHandler GetPlatformViewHandlerFromVisualElement(VisualElement viewElement)
    {
        return viewElement.ToHandler(Application.Current?.Handler?.MauiContext);
    }

}