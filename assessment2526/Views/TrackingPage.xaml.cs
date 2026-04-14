using assessment2526.ViewModels;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;


namespace assessment2526.Views;


public partial class TrackingPage : ContentPage
{
    private TrackingViewModel _viewModel;
    public TrackingPage()
	{
		InitializeComponent();
        _viewModel = new TrackingViewModel();
        BindingContext = _viewModel;
    }

 protected override void OnAppearing()
    {
        base.OnAppearing();

        UpdateMap();
    }

    private async void UpdateMap()
    {
        await Task.Delay(1000); // wait for location

        if (_viewModel.Latitude != 0 && _viewModel.Longitude != 0)
        {
            var location = new Location(_viewModel.Latitude, _viewModel.Longitude);

            MapView.MoveToRegion(
                MapSpan.FromCenterAndRadius(location, Distance.FromKilometers(1)));
        }
    }
}