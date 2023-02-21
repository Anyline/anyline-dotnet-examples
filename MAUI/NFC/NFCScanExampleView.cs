namespace Anyline.Examples.MAUI.NFC;

/// <summary>
/// This ContentView is rendered natively in each individual platform. 
/// The scanning results are received via MessagingCenter.
/// </summary>
public class NFCScanExampleView : View
{
    public string JSONConfigPath = "";
    private MyScanResults myScanResults = new MyScanResults();

    public NFCScanExampleView(string jsonConfigPath, Action<MyScanResults> onResult, Action<string> onError)
    {
        BackgroundColor = Colors.Black;
        JSONConfigPath = jsonConfigPath;

        // Listens for the results of the MRZ Scan Result
        MessagingCenter.Subscribe<Microsoft.Maui.Controls.Application, MyMRZScanResults>(this, "MRZ_READING_DONE", (s, results) =>
        {
            MessagingCenter.Unsubscribe<Microsoft.Maui.Controls.Application, MyMRZScanResults>(this, "MRZ_READING_DONE");
            myScanResults.MRZResults = results;
        });

        // Listens for the results of the NFC Scan Result
        MessagingCenter.Subscribe<Microsoft.Maui.Controls.Application, MyNFCScanResults>(this, "NFC_SCAN_FINISHED_SUCCESS", (s, results) =>
        {
            MessagingCenter.Unsubscribe<Microsoft.Maui.Controls.Application, MyNFCScanResults>(this, "NFC_SCAN_FINISHED_SUCCESS");
            myScanResults.NFCResults = results;

            onResult?.Invoke(myScanResults);
        });

        // Listens for errors on the NFC Scanning Process
        MessagingCenter.Subscribe<Microsoft.Maui.Controls.Application, string>(this, "NFC_SCAN_FINISHED_ERROR", (s, message) =>
        {
            MessagingCenter.Unsubscribe<Microsoft.Maui.Controls.Application, string>(this, "NFC_SCAN_FINISHED_ERROR");
            onError?.Invoke(message);
        });
    }
}
