using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace assessment2526.ViewModels;

public partial class TrackingViewModel : INotifyPropertyChanged
{
    //Fields
    private bool _isRunning = false;
    private int _seconds = 0;

    //Location fields
    private double _latitude;
    private double _longitude;

    private readonly IConnectivity _connectivity;
    private readonly IGeolocation _geolocation;
    
    //Properties

    private string _timeText = "Time: 00:00:00";
    public string TimeText
    {
        get => _timeText;
        set { _timeText = value; OnPropertyChanged(); }
    }


    private string _distanceText = "Distance: 0.00 km";
    public string DistanceText
    {
        get => _distanceText;
        set { _distanceText = value; OnPropertyChanged(); }
    }


    private string _speedText = "Speed: 0.0 km/h";
    public string SpeedText
    {
        get => _speedText;
        set { _speedText = value; OnPropertyChanged(); }
    }


    private string _statusText = "Status: Waiting to start...";
    public string StatusText
    {
        get => _statusText;
        set { _statusText = value; OnPropertyChanged(); }
    }

    public double Latitude
    {
        get => _latitude;
        set { _latitude = value; OnPropertyChanged(); }
    }

    public double Longitude
    {
        get => _longitude;
        set { _longitude = value; OnPropertyChanged(); }
    }

    //Commands for start and stop buttons
    public ICommand StartCommand { get; }
    public ICommand StopCommand { get; }


    public TrackingViewModel(IConnectivity connectivity, IGeolocation geolocation)
    {
        StartCommand = new Command(async () => await StartRun());
        StopCommand = new Command(StopRun);

        _connectivity = connectivity;
        _geolocation = geolocation;
        LoadLocation();
    }

    //Start run
    private async Task StartRun()
    {
        if (_isRunning)
            return;

        _isRunning = true;
        StatusText = "Status: Running...";

        StartTimer();
        await SpeakAsync("Run started");

        //when distance updates text-to-speech to user
        await SpeakAsync("Distance updated");
    }

    //Stop run
    private void StopRun()
    {
        _isRunning = false;
        StatusText = "Status: Stopped";
    }

    //Timer logic
    private void StartTimer()
    {
        var dispatcher = Application.Current?.Dispatcher;

        if (dispatcher == null)
            return;

        dispatcher.StartTimer(TimeSpan.FromSeconds(1), () =>
        {
            if (!_isRunning)
                return false;

            _seconds++;

            TimeText = $"Time: {TimeSpan.FromSeconds(_seconds):hh\\:mm\\:ss}";

            return true;
        });
    }

    private async void LoadLocation()
    {
        var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

        if (status != PermissionStatus.Granted)
        {
            StatusText = "Location permission denied";
            return;
        }
        try
        {
            //check connectivity
            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert(
                    "No connectivity!",
                    "Please check internet and try again.",
                    "OK");

                StatusText = "No internet connection";
                return;
            }

            //get last known location first
            var location = await _geolocation.GetLastKnownLocationAsync();

            if (location == null)
            {
                location = await _geolocation.GetLocationAsync(
                    new GeolocationRequest
                    {
                        DesiredAccuracy = GeolocationAccuracy.Medium,
                        Timeout = TimeSpan.FromSeconds(10)
                    });
            }

            if (location != null)
            {
                Latitude = location.Latitude;
                Longitude = location.Longitude;

                StatusText = $"Lat: {Latitude}, Lon: {Longitude}";
            }
            else
            {
                StatusText = "Unable to get location";
            }
        }
        catch (Exception ex)
        {
            StatusText = "Error getting location";
            System.Diagnostics.Debug.WriteLine($"Location error: {ex.Message}");
        }

    }


    //text-to-speech function
    public async Task SpeakAsync(string text)
    {
        await TextToSpeech.Default.SpeakAsync(text);
    }


    //Property change notification
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}