namespace Lidarr.Recommendations.Config;

public static class SettingKeys
{
    public const string PluginId = "lidarr.recommendations";

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "Organizes setting keys by category")]
    public static class Toggles
    {
        public const string OfflineOnly = "offlineOnly";
        public const string ListenBrainz = "enableListenBrainz";
        public const string MusicBrainz = "enableMusicBrainz";
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "Organizes setting keys by category")]
    public static class Sliders
    {
        public const string Novelty = "novelty";
        public const string MinPopularity = "minPopularity";
    }
}
