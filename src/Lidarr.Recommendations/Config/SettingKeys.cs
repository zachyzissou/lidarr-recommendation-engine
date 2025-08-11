namespace Lidarr.Recommendations.Config;

public static class SettingKeys
{
    public const string PluginId = "lidarr.recommendations";

    public static class Toggles
    {
        public const string OfflineOnly = "offlineOnly";
        public const string ListenBrainz = "enableListenBrainz";
        public const string MusicBrainz = "enableMusicBrainz";
    }

    public static class Sliders
    {
        public const string Novelty = "novelty";
        public const string MinPopularity = "minPopularity";
    }
}
