namespace Lidarr.Recommendations.Domain;

public sealed class ArtistProfile
{
    public string Id { get; set; } = ""; // Lidarr/MBID
    public string Name { get; set; } = "";
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Domain object requires mutable collections for serialization")]
    public HashSet<string> Tags { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public string? Country { get; set; }
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Domain object requires mutable collections for serialization")]
    public HashSet<int> Decades { get; set; } = new();
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Domain object requires mutable collections for serialization")]
    public HashSet<string> Labels { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Domain object requires mutable collections for serialization")]
    public HashSet<string> RelatedArtistIds { get; set; } = new(StringComparer.OrdinalIgnoreCase); // Local relations if available
    public double PopularityProxy { get; set; }
}
