using Lidarr.Recommendations.Domain;

namespace Lidarr.Recommendations.Services;

// Adapts host APIs to plugin. Implement using actual plugin SDK services.
public interface ILibraryAdapter
{
    Task<IReadOnlyList<ArtistProfile>> GetArtistsAsync(CancellationToken ct);
    Task<IReadOnlyList<AlbumProfile>> GetAlbumsByArtistAsync(string artistId, CancellationToken ct);
    Task<HashSet<string>> GetOwnedArtistIdsAsync(CancellationToken ct);
    Task<DateTimeOffset?> GetMostRecentAdditionAsync(string artistId, CancellationToken ct);
}
