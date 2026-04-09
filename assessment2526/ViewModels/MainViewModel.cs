using System.Windows.Input;
using assessment2526.Views;

namespace assessment2526.ViewModels;

public class MainViewModel
{
    public ICommand StartRunCommand { get; }

    public MainViewModel()
    {
        StartRunCommand = new Command(async () =>
        {
            await Application.Current.MainPage.Navigation.PushAsync(new TrackingPage());
        });
    }
}