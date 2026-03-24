using assessment2526.Views;
namespace assessment2526;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    // Navigate to Tracking Page
    private async void OnStartRun(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new TrackingPage());
    }
}