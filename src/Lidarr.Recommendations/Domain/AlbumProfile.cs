namespace Lidarr.Recommendations.Domain;

public sealed class AlbumProfile
{
    public string Id { get; set; } = ""; // release-group or album id
    public string ArtistId { get; set; } = "";
    public string Title { get; set; } = "";
    public int? Year { get; set; }
    public bool IsOwned { get; set; }
    public bool IsLive { get; set; }
    public bool IsCompilation { get; set; }
    public bool IsEP { get; set; }
    public bool IsRemaster { get; set; }
    public bool IsUpcoming { get; set; }
}
