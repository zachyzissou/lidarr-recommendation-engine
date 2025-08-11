namespace Lidarr.Recommendations.Domain;

public sealed class ArtistProfile
{
    public string Id { get; set; } = ""; // Lidarr/MBID
    public string Name { get; set; } = "";
    public HashSet<string> Tags { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public string? Country { get; set; }
    public HashSet<int> Decades { get; set; } = new();
    public HashSet<string> Labels { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public HashSet<string> RelatedArtistIds { get; set; } = new(StringComparer.OrdinalIgnoreCase); // Local relations if available
    public double PopularityProxy { get; set; } = 0.0; // 0..1 proxy (local)
}
