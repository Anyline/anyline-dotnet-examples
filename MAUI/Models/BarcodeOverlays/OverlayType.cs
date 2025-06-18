using Anyline.SDK.NET.Common.ScanView.BarcodeOverlays;

namespace Anyline.Examples.MAUI.Models.BarcodeOverlays;

public class OverlayType(double barcodeArea)
{
    private const int MaxSmallArea = 12000;
    public readonly bool IsSmall = (barcodeArea < MaxSmallArea);
    
    public static OverlayType GetOverlayTypeFromVisibleBarcode(CommonVisibleBarcode commonVisibleBarcode, bool isSelected)
    {
        var barcodeArea = commonVisibleBarcode.GetArea();
        if (isSelected)
        {
            return new OverlayTypeSelected(barcodeArea);
        }
        return new OverlayTypeNotSelected(barcodeArea);
    }    
    
    public string GetImageSource()
    {
        if (this is OverlayTypeSelected)
        {
            return IsSmall ? "ic_barcode_overlay_far_green.png" : "ic_barcode_overlay_checkmark_green.png";
        }
        else
        {
            return IsSmall ? "ic_barcode_overlay_far_blue.png" : "ic_barcode_overlay_plus_blue.png";
        }
    }    
}
internal sealed class OverlayTypeSelected(double barcodeArea) : OverlayType(barcodeArea);
internal sealed class OverlayTypeNotSelected(double barcodeArea) : OverlayType(barcodeArea);