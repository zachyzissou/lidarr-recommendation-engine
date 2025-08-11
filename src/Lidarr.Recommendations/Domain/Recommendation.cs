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
    public string? MusicBrainzUrl { get; set; }
    public string? ListenBrainzUrl { get; set; }

    public string[] SuggestedActions { get; set; } = Array.Empty<string>(); // "Add Artist","Add Album","Add to Wanted","Hide","Pin"
}
