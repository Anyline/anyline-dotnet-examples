using Anyline.SDK.NET.Common.ScanView.BarcodeOverlays;

namespace Anyline.Examples.MAUI.Models.BarcodeOverlays;

public class CommonBarcodeOverlayListenerImpl: ICommonBarcodeOverlayListener
{
    private readonly Dictionary<string, CommonVisibleBarcode> _selectedBarcodeMap = new();
    
    public List<CommonOverlayView> OnCreate(CommonVisibleBarcode commonVisibleBarcode)
    {
        /*
         * OnCreate() is called every time a barcode that is not contained
         * on current overlays is discovered. Must return a list of Views
         * that will be placed near the barcode.
         */        
        return new BarcodeOverlayViewList(this, commonVisibleBarcode);
    }
    
    public void OnUpdate(List<CommonOverlayView> overlayViews, CommonVisibleBarcode commonVisibleBarcode) {
        /*
         * OnUpdate() is called every time a previously detected barcode needs to be repositioned.
         */
        var oldOverlayType = (overlayViews as BarcodeOverlayViewList).OverlayType;
        var newOverlayType =
            OverlayType.GetOverlayTypeFromVisibleBarcode(commonVisibleBarcode, IsSelected(commonVisibleBarcode.Value));
        if (oldOverlayType.IsSmall != newOverlayType.IsSmall) {
            // calling Invalidate() makes the overlay to be disposed
            // and a new OnCreate() will be called for a new view instance
            commonVisibleBarcode.Invalidate();
        }        
    }    
    
    public long GetClearTimeoutMills()
    {
        return 500L;
    }    
    
    public bool IsSelected(string barcodeValue)
    {
        return _selectedBarcodeMap.ContainsKey(barcodeValue);
    }    
    
    public void SetSelected(bool selected, CommonVisibleBarcode commonVisibleBarcode) {
        if (selected)
        {
            //initialize barcodeImage to preserve native image object
            var barcodeImage = commonVisibleBarcode.BarcodeImageMemoryStream.Value; 
            
            _selectedBarcodeMap[commonVisibleBarcode.Value] = commonVisibleBarcode;
        } else
        {
            _selectedBarcodeMap.Remove(commonVisibleBarcode.Value);
        }
    }    
    
    public List<CommonVisibleBarcode> GetSelectedBarcodes()
    {
        return _selectedBarcodeMap.Values.ToList();
    }
    
    public void ResetSelectedBarcodes()
    {
        _selectedBarcodeMap.Clear();
    }

}