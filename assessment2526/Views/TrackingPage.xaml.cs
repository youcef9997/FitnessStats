using assessment2526.ViewModels;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;


namespace assessment2526.Views;


public partial class TrackingPage : ContentPage
{
    private TrackingViewModel? _viewModel;
    public TrackingPage()
	{
		InitializeComponent();

        _viewModel = new TrackingViewModel(Connectivity.Current, Geolocation.Default);
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        //small delay to let UI load
        await Task.Delay(300);

        //UpdateMap();
    }
}