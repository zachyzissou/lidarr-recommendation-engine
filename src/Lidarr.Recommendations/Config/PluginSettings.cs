namespace Lidarr.Recommendations.Config;

public sealed class PluginSettings
{
    public bool OfflineOnly { get; set; } = true;
    public bool EnableListenBrainz { get; set; } = false;
    public bool EnableMusicBrainz { get; set; } = false;

    // Sliders
    public double Novelty { get; set; } = 0.5;           // 0..1 (0 = Familiar, 1 = Novel)
    public double MinPopularity { get; set; } = 0.2;     // 0..1

    // Filters
    public string[] IncludeGenres { get; set; } = Array.Empty<string>();
    public string[] ExcludeGenres { get; set; } = Array.Empty<string>();
    public int[] IncludeYears { get; set; } = Array.Empty<int>();
    public int[] ExcludeYears { get; set; } = Array.Empty<int>();
    public string[] IncludeCountries { get; set; } = Array.Empty<string>();
    public string[] ExcludeCountries { get; set; } = Array.Empty<string>();
    public bool HideLiveAndCompilations { get; set; } = true;

    // Behavior
    public bool AutoTagAccepted { get; set; } = true;
    public bool PreferLossless { get; set; } = false;

    // Weights
    public ScoreWeights Weights { get; set; } = new();
}
