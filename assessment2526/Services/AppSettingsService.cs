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

        private const string FontSizeKey = "FontSizeOption";

        public static string FontSizeOption
        {
            get => Preferences.Get(FontSizeKey, "Medium");
            set => Preferences.Set(FontSizeKey, value);
        }
    }
}
