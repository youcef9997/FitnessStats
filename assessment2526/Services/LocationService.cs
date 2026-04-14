using Microsoft.Maui.Devices.Sensors;

namespace assessment2526.Services;

public class LocationService
{
    public async Task<Location?> GetCurrentLocationAsync()
    {
        try
        {
            var request = new GeolocationRequest(
                GeolocationAccuracy.Best,
                TimeSpan.FromSeconds(10));

            var location = await Geolocation.GetLocationAsync(request);

            return location;
        }
        catch (Exception)
        {
            return null;
        }
    }
}