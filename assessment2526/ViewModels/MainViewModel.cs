using System.Windows.Input;
using assessment2526.Views;

namespace assessment2526.ViewModels;

public class MainViewModel
{
    public ICommand StartRunCommand { get; }
    public ICommand GoToSettingsCommand { get; }
    public MainViewModel()
    {
        StartRunCommand = new Command(async () =>
        {
            var page = Application.Current?.Windows[0]?.Page;

            if (page != null)
            {
                await page.Navigation.PushAsync(new TrackingPage());
            }
        });

        GoToSettingsCommand = new Command(async () =>
        {
            var page = Application.Current?.Windows[0]?.Page;

            if (page != null)
            {
                await page.Navigation.PushAsync(new SettingsPage());
            }
        });
    }
}