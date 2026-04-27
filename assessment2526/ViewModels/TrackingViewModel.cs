using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Devices;
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

    //Accelerometer fields
    private bool _isAccelerometerRunning = false;
    private bool _isPaused = false;
    private int _noMovementSeconds = 0;
    //Seconds of no movement to consider pause
    private const int PauseThreshold = 5; 


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

    private bool _isStatsVisible = true;
    public bool IsStatsVisible
    {
        get => _isStatsVisible;
        set { _isStatsVisible = value; OnPropertyChanged(); }
    }

    private bool _isMapVisible = false;
    public bool IsMapVisible
    {
        get => _isMapVisible;
        set { _isMapVisible = value; OnPropertyChanged(); }
    }
    public Location? CurrentLocation
    { get; set;  }
    //Commands for start and stop buttons
    public ICommand StartCommand { get; }
    public ICommand StopCommand { get; }

    //Command for toggle between stats and map
    public ICommand ToggleViewCommand => new Command(() =>
    {
        IsStatsVisible = !IsStatsVisible;
        IsMapVisible = !IsMapVisible;
    });

    public TrackingViewModel(IConnectivity connectivity, IGeolocation geolocation)
    {
        StartCommand = new Command(async () => await StartRun());
        StopCommand = new Command(async () => await StopRun());

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

        //haptic feedback on start
        Vibrate();

        StartTimer();
        await SpeakAsync("Run started");

        //when distance updates text-to-speech to user
        await SpeakAsync($"Distance {DistanceText}");

        //start accelerometer
        StartAccelerometer();
    }

    //Stop run
    private async Task StopRun()
    {
        _isRunning = false;
        StatusText = "Status: Stopped";

        // stronger haptic feedback on stop
        Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(600));

        await SpeakAsync("Run stopped");

        //stop accelerometer
        StopAccelerometer();
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

    //location with connectivity check
    private async void LoadLocation()
    {   
        //location permission request
        var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

        if (status != PermissionStatus.Granted)
        {
            StatusText = "Location permission denied";
            return;
        }
        try
        {
            //check network connectivity
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
                CurrentLocation = location;
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


    //haptic feedback function
    public void Vibrate()
    {
        Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(300));

    }

    private void StartAccelerometer()
    {
        if (_isAccelerometerRunning)
            return;

        Accelerometer.ReadingChanged += OnAccelerometerChanged;
        Accelerometer.Start(SensorSpeed.UI);

        _isAccelerometerRunning = true;
    }

    private void StopAccelerometer()
    {
        if (!_isAccelerometerRunning)
            return;

        Accelerometer.Stop();
        Accelerometer.ReadingChanged -= OnAccelerometerChanged;

        _isAccelerometerRunning = false;
    }

    private void OnAccelerometerChanged(object? sender, AccelerometerChangedEventArgs e)
    {
    var data = e.Reading;

    double movement =
             Math.Sqrt(
        data.Acceleration.X * data.Acceleration.X +
        data.Acceleration.Y * data.Acceleration.Y +
        data.Acceleration.Z * data.Acceleration.Z);

        // threshold to detect movement
        if (movement > 1.2)
        {
            _noMovementSeconds = 0;

            if (_isPaused)
            {
                _isPaused = false;
                StatusText = "Status: Running (resumed)";
                StartTimer();
            }
        }
        else
        {
            _noMovementSeconds++;

            if (_noMovementSeconds >= PauseThreshold && !_isPaused)
            {
                _isPaused = true;
                StatusText = "Status: Paused (no movement)";
               
            }
        }
    }

    // debug to test accelerometer data
    //void OnAccelerometerChanged(object? sender, AccelerometerChangedEventArgs e)
    //{
      //  var data = e.Reading;

       // double x = data.Acceleration.X;
      //  double y = data.Acceleration.Y;
       // double z = data.Acceleration.Z;

      //  double total = Math.Sqrt(x * x + y * y + z * z);

       // StatusText = $"Move: {total:F2}";
   // }

    //Property change notification
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}