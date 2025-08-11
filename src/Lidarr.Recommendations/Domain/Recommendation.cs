namespace Lidarr.Recommendations.Domain;

public sealed class Recommendation
{
    public string Id { get; set; } = "";       // artistId or albumId
    public string? ParentId { get; set; }      // for AlbumGap: artistId
    public RecommendationType Type { get; set; }
    public string Title { get; set; } = "";
    public string? Subtitle { get; set; }
    public double Score { get; set; }
    public Reason Reason { get; set; } = new();

    // Links and actions
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:URI-like properties should not be strings", Justification = "URLs are provided by external APIs as strings and may not always be valid URIs")]
    public string? MusicBrainzUrl { get; set; }
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:URI-like properties should not be strings", Justification = "URLs are provided by external APIs as strings and may not always be valid URIs")]
    public string? ListenBrainzUrl { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Domain object API requires array property")]
    public string[] SuggestedActions { get; set; } = Array.Empty<string>(); // "Add Artist","Add Album","Add to Wanted","Hide","Pin"
}
