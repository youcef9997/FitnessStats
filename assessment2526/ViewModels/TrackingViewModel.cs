using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Devices.Sensors;
using assessment2526.Services;

namespace assessment2526.ViewModels;

public class TrackingViewModel : INotifyPropertyChanged
{
    //Fields
    private string _timeText = "Time: 00:00:00";
    private string _distanceText = "Distance: 0.00 km";
    private string _speedText = "Speed: 0.0 km/h";
    private string _statusText = "Status: Waiting to start...";

    private bool _isRunning = false;
    private int _seconds = 0;

    //Location fields
    private double _latitude;
    private double _longitude;

    private readonly LocationService _locationService;

    //Properties 
    public string TimeText
    {
        get => _timeText;
        set { _timeText = value; OnPropertyChanged(); }
    }

    public string DistanceText
    {
        get => _distanceText;
        set { _distanceText = value; OnPropertyChanged(); }
    }

    public string SpeedText
    {
        get => _speedText;
        set { _speedText = value; OnPropertyChanged(); }
    }

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

    public TrackingViewModel()
    {
        StartCommand = new Command(StartRun);
        StopCommand = new Command(StopRun);

        _locationService = new LocationService();
        LoadLocation();
    }

    //Start run
    private void StartRun()
    {
        if (_isRunning)
            return;

        _isRunning = true;
        StatusText = "Status: Running...";

        StartTimer();
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
        try
        {
            var location = await _locationService.GetCurrentLocationAsync();

            if (location != null)
            {
                Latitude = location.Latitude;
                Longitude = location.Longitude;

                StatusText = "Location loaded";
            }
            else
            {
                StatusText = "Unable to get location";
            }
        }
        catch (Exception)
        {
            StatusText = "Error getting location";
        }
    }

    //Property change notification
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}