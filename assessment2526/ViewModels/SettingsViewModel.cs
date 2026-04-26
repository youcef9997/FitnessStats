namespace assessment2526.ViewModels;
using assessment2526.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public partial class SettingsViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    void OnPropertyChanged([CallerMemberName] string name = null!)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public ObservableCollection<string> Themes { get; } =
        new() { "System", "Light", "Dark" };

    private string _selectedTheme = "System"; 

    public string SelectedTheme
    {
        get => _selectedTheme;
        set
        {
            if (_selectedTheme != value)
            {
                _selectedTheme = value;
                OnPropertyChanged();
                ApplyTheme();
            }
        }
    }

    public SettingsViewModel()
    {
        SelectedTheme = ThemeService.GetCurrentTheme();
    }

    private void ApplyTheme()
    {
        ThemeService.ApplyTheme(SelectedTheme);
    }
}