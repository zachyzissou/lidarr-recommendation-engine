using Lidarr.Recommendations.Domain;
using Microsoft.Extensions.Logging;

namespace Lidarr.Recommendations.Services.Providers;

public sealed class ListenBrainzProvider
{
    private readonly ILogger<ListenBrainzProvider> _logger;

    public ListenBrainzProvider(ILogger<ListenBrainzProvider> logger)
    {
        _logger = logger;
    }

    public Task<IReadOnlyList<(ArtistProfile artist, double coListen)>> GetCoListensAsync(string artistId, CancellationToken ct)
    {
        // Stub for first pass; to be implemented behind feature toggle
        _logger.LogDebug("ListenBrainzProvider: stub called for {ArtistId}", artistId);
        return Task.FromResult<IReadOnlyList<(ArtistProfile, double)>>(Array.Empty<(ArtistProfile, double)>());
    }
}
