using assessment2526.ViewModels;
namespace assessment2526.Views;

public partial class TrackingPage : ContentPage
{
	public TrackingPage()
	{
		InitializeComponent();
        BindingContext = new TrackingViewModel();
    }
}