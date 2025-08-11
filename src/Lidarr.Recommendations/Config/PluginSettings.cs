namespace Lidarr.Recommendations.Config;

public sealed class PluginSettings
{
    public bool OfflineOnly { get; set; } = true;
    public bool EnableListenBrainz { get; set; }
    public bool EnableMusicBrainz { get; set; }

    // Sliders
    public double Novelty { get; set; } = 0.5;           // 0..1 (0 = Familiar, 1 = Novel)
    public double MinPopularity { get; set; } = 0.2;     // 0..1

    // Filters
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Plugin configuration API requires array properties")]
    public string[] IncludeGenres { get; set; } = Array.Empty<string>();
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Plugin configuration API requires array properties")]
    public string[] ExcludeGenres { get; set; } = Array.Empty<string>();
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Plugin configuration API requires array properties")]
    public int[] IncludeYears { get; set; } = Array.Empty<int>();
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Plugin configuration API requires array properties")]
    public int[] ExcludeYears { get; set; } = Array.Empty<int>();
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Plugin configuration API requires array properties")]
    public string[] IncludeCountries { get; set; } = Array.Empty<string>();
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Plugin configuration API requires array properties")]
    public string[] ExcludeCountries { get; set; } = Array.Empty<string>();
    public bool HideLiveAndCompilations { get; set; } = true;

    // Behavior
    public bool AutoTagAccepted { get; set; } = true;
    public bool PreferLossless { get; set; }

    // Weights
    public ScoreWeights Weights { get; set; } = new();
}
