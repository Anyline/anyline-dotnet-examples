using Anyline.Examples.MAUI.Models;
using Anyline.SDK.NET.Common;
using Anyline.SDK.NET.Common.Result;
using Anyline.SDK.NET.Common.ScanView.BarcodeOverlays;
using Newtonsoft.Json.Linq;

namespace Anyline.Examples.MAUI;

public partial class ResultsPage : ContentPage
{
    /// <summary>
    /// This page is responsible for displaying the scan results. 
    /// It is built in a generic way to support any scan mode, but in your use-case, it is recommended to work only with a strongly typed object in the MAUI layer.
    /// </summary>
    /// <param name="scanResults"></param>
    /// <param name="scanMode">Object containing the Name of the ScanMode and the JSON config file path (used for re-initializing the ScanView page)</param>
    public ResultsPage(ScanResult[] scanResults, AnylineScanMode scanMode)
    {
        InitializeComponent();

        Task.Run(() => ShowResults(scanResults));

        SetupButtons(scanMode);
    }
    
    /// <summary>
    /// This page is responsible for displaying the barcode results. 
    /// It is built in a generic way to support any scan mode, but in your use-case, it is recommended to work only with a strongly typed object in the MAUI layer.
    /// </summary>    
    /// <param name="barcodes"></param>
    /// <param name="scanMode">Object containing the Name of the ScanMode and the JSON config file path (used for re-initializing the ScanView page)</param>
    public ResultsPage(CommonVisibleBarcode[] barcodes, AnylineScanMode scanMode)
    {
        InitializeComponent();

        Task.Run(() => ShowResults(barcodes));

        SetupButtons(scanMode);
    }

    private void SetupButtons(AnylineScanMode scanMode)
    {
        btHome.Clicked += async (s, e) => await Navigation.PopToRootAsync();
        btScanAgain.Clicked += (s, e) =>
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                Navigation.InsertPageBefore(new MyScanningWithAnylinePage(scanMode), this);
                await Navigation.PopAsync();
            });
        };
    }

    private void ShowResults(ScanResult[] scanResults)
    {
        View viewResults = CreateResultView(scanResults);
        MainThread.BeginInvokeOnMainThread(() =>
        {
            cvContent.Content = viewResults;

            // workaround for the ScrollView height misscalculation on iOS
            // when dynamically loading content into the layout.
            var scrollView = new ScrollView();
            scrollView.Content = Content;
            Content = scrollView;
        });
    }
    
    private void ShowResults(CommonVisibleBarcode[] barcodes)
    {
        View viewResults = CreateResultView(barcodes);
        MainThread.BeginInvokeOnMainThread(() =>
        {
            cvContent.Content = viewResults;

            // workaround for the ScrollView height misscalculation on iOS
            // when dynamically loading content into the layout.
            var scrollView = new ScrollView();
            scrollView.Content = Content;
            Content = scrollView;
        });
    }    

    private static void AddImageView(StackLayout container, string labelText, MemoryStream imageMemoryStream)
    {
        var formattedString = new FormattedString();
        formattedString.Spans.Add(new Span() { Text = labelText, TextColor = Color.FromArgb("32ADFF"), FontSize = 15, FontAttributes = FontAttributes.Bold });
        container.Children.Add(new Label() { FormattedText = formattedString });
            
        var img = new Image()
        {
            Aspect = Aspect.AspectFit,
            HeightRequest = 100,
            Source = ImageSource.FromStream(() => imageMemoryStream)
        };
        container.Children.Add(img);
    }

    private static Grid CreateGridViewWithContent(View contentView)
    {
        Grid grContent = new Grid()
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(0.3)),
                new ColumnDefinition(GridLength.Star)
            }
        };

        grContent.Add(new BoxView { Color = Colors.Gray }, 0, 0);
        grContent.Add(contentView, 1, 0);

        return grContent;  
    }

    private static View CreateResultView(ScanResult[] scanResults)
    {
        StackLayout slItemResults = new StackLayout() { Padding = new Thickness(5, 0, 0, 0) };
        foreach (var scanResult in scanResults)
        {
            AddImageView(slItemResults, "Cutout Image", scanResult.CutoutImageMemoryStream.Value);

            var faceImageMemoryStream = scanResult.FaceImageMemoryStream.Value;
            if (faceImageMemoryStream != null)
            {
                AddImageView(slItemResults, "Face Image", faceImageMemoryStream);
            }
            
            slItemResults.Children.Add(CreateResultView(JObject.Parse(scanResult.PluginResult.ToJson())));
        }

        return CreateGridViewWithContent(slItemResults);        
    }
    
    private static View CreateResultView(CommonVisibleBarcode[] barcodes)
    {
        StackLayout slItemResults = new StackLayout() { Padding = new Thickness(5, 0, 0, 0) };
        foreach (var barcode in barcodes)
        {
            var barcodeImage = barcode.BarcodeImageMemoryStream.Value;
            if (barcodeImage != null)
            {
                AddImageView(slItemResults, "Barcode Image", barcodeImage);    
            }
            var jsonResult = barcode.Barcode.ToJson();
            slItemResults.Children.Add(CreateResultView(JObject.Parse(jsonResult)));
        }

        return CreateGridViewWithContent(slItemResults);        
    }    

    private static View CreateResultView(JArray jArray)
    {
        StackLayout slItemResults = new StackLayout() { Padding = new Thickness(5, 0, 0, 0) };

        foreach (var item in jArray)
        {
            if (item is JObject subItems)
            {
                slItemResults.Children.Add(CreateResultView(subItems));
            }
            else if (item is JArray array)
            {
                slItemResults.Children.Add(CreateResultView(array));
            }            
            else
            {
                slItemResults.Children.Add(new Label { Text = item.ToString(), TextColor = Colors.White, FontAttributes = FontAttributes.Bold, FontSize = 13 });    
            }
        }

        return CreateGridViewWithContent(slItemResults);            
    }

    private static View CreateResultView(JObject jObject)
    {
        StackLayout slItemResults = new StackLayout() { Padding = new Thickness(5, 0, 0, 0) };

        foreach (var item in jObject)
        {
            var formmattedString = new FormattedString();
            formmattedString.Spans.Add(new Span() { Text = item.Key, TextColor = Color.FromArgb("32ADFF"), FontSize = 15, FontAttributes = FontAttributes.Bold });

            slItemResults.Children.Add(new Label() { FormattedText = formmattedString });

            if (item.Value is JObject subItems)
            {
                slItemResults.Children.Add(CreateResultView(subItems));
            }
            else if (item.Value is JArray array)
            {
                slItemResults.Children.Add(CreateResultView(array));
            }
            else
            {
                slItemResults.Children.Add(new Label { Text = item.Value.ToString(), TextColor = Colors.White, FontAttributes = FontAttributes.Bold, FontSize = 17 });
            }
        }

        return CreateGridViewWithContent(slItemResults);    
    }
}
