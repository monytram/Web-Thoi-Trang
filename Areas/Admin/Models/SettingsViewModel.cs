namespace WebThoiTrang.Areas.Admin.Models
{
    public class SettingsViewModel
    {
        public Dictionary<string, string> Settings { get; set; } = new();

        public string GetValue(string key)
        {
            return Settings != null && Settings.TryGetValue(key, out string value)
                ? value
                : string.Empty;
        }

        public DateTime? GetDateTime(string key)
        {
            if (Settings != null && Settings.TryGetValue(key, out string value))
            {
                return DateTime.TryParse(value, out DateTime result) ? result : null;
            }
            return null;
        }
    }
}
