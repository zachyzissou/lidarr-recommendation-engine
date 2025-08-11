using Lidarr.Recommendations.Domain;
using Microsoft.Extensions.Logging;

namespace Lidarr.Recommendations.Services;

public sealed class LibraryAdapter : ILibraryAdapter
{
    private readonly ILogger<LibraryAdapter> _logger;

    public LibraryAdapter(ILogger<LibraryAdapter> logger)
    {
        _logger = logger;
    }

    public Task<IReadOnlyList<ArtistProfile>> GetArtistsAsync(CancellationToken ct)
    {
        // TODO: Implement via Lidarr plugin SDK (query artists, tags, relations).
        // Baseline: return minimal set to ensure engine doesn't fail.
        _logger.LogDebug("GetArtistsAsync: using baseline stub");
        return Task.FromResult<IReadOnlyList<ArtistProfile>>(Array.Empty<ArtistProfile>());
    }

    public Task<IReadOnlyList<AlbumProfile>> GetAlbumsByArtistAsync(string artistId, CancellationToken ct)
    {
        _logger.LogDebug("GetAlbumsByArtistAsync({ArtistId}): baseline stub", artistId);
        return Task.FromResult<IReadOnlyList<AlbumProfile>>(Array.Empty<AlbumProfile>());
    }

    public Task<HashSet<string>> GetOwnedArtistIdsAsync(CancellationToken ct)
    {
        _logger.LogDebug("GetOwnedArtistIdsAsync: baseline stub");
        return Task.FromResult(new HashSet<string>(StringComparer.OrdinalIgnoreCase));
    }

    public Task<DateTimeOffset?> GetMostRecentAdditionAsync(string artistId, CancellationToken ct)
    {
        _logger.LogDebug("GetMostRecentAdditionAsync({ArtistId}): baseline stub", artistId);
        return Task.FromResult<DateTimeOffset?>(null);
    }
}
