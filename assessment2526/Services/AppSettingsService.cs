using Microsoft.Maui.Storage;

namespace assessment2526.Services
{
    public static class AppSettingsService
    {
        private const string AutoPauseKey = "AutoPauseEnabled";

        public static bool AutoPauseEnabled
        {
            get => Preferences.Get(AutoPauseKey, true);
            set => Preferences.Set(AutoPauseKey, value);
        }
    }
}
