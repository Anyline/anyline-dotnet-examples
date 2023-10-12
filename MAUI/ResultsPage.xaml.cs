using Anyline.Examples.MAUI.Models;
using Microsoft.Maui.Controls.Shapes;

namespace Anyline.Examples.MAUI;

public partial class ResultsPage : ContentPage
{
    /// <summary>
    /// This page is reponsible for displaying the scan results. 
    /// It is built in a generic way to support any scan mode, but in your use-case, it is recommended to work only with a strongly typed object in the MAUI layer.
    /// </summary>
    /// <param name="results"></param>
    /// <param name="scanMode">Object containing the Name of the ScanMode and the JSON config file path (used for re-initializing the ScanView page)</param>
    public ResultsPage(Dictionary<string, object> results, AnylineScanMode scanMode)
    {
        InitializeComponent();

        Task.Run(() => ShowResults(results));

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

    private void ShowResults(Dictionary<string, object> results)
    {
        View viewResults = CreateResultView(results);
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


    private View CreateResultView(Dictionary<string, object> dict)
    {
        StackLayout slItemResults = new StackLayout() { Padding = new Thickness(5, 0, 0, 0) };

        foreach (var item in dict)
        {
            string[] name_type = item.Key.Split(' ');

            var formmattedString = new FormattedString();
            formmattedString.Spans.Add(new Span() { Text = name_type[0] + " ", TextColor = Color.FromArgb("32ADFF"), FontSize = 15, FontAttributes = FontAttributes.Bold });
            formmattedString.Spans.Add(new Span() { Text = name_type[1], TextColor = Color.FromArgb("32ADFF"), FontSize = 10 });

            slItemResults.Children.Add(new Label() { FormattedText = formmattedString });

            if (item.Value is byte[] imageBytes)
            {
                var img = new Image()
                {
                    Aspect = Aspect.AspectFit,
                    HeightRequest = 100,
                    Source = ImageSource.FromStream(() => new MemoryStream(imageBytes))
                };

                slItemResults.Children.Add(img);
            }
            else if (item.Value is Dictionary<string, object> subItems)
            {
                slItemResults.Children.Add(CreateResultView(subItems));
            }
            else
            {
                slItemResults.Children.Add(new Label { Text = item.Value.ToString(), TextColor = Colors.White, FontAttributes = FontAttributes.Bold, FontSize = 17 });
            }
        }

        Grid grContent = new Grid()
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(0.3)),
                new ColumnDefinition(GridLength.Star)
            }
        };

        grContent.Add(new BoxView { Color = Colors.Gray }, 0, 0);
        grContent.Add(slItemResults, 1, 0);

        return grContent;
    }
}
