using Lidarr.Recommendations.Domain;
using Microsoft.Extensions.Logging;

namespace Lidarr.Recommendations.Services.Providers;

public sealed class MusicBrainzProvider
{
    private readonly ILogger<MusicBrainzProvider> _logger;

    public MusicBrainzProvider(ILogger<MusicBrainzProvider> logger)
    {
        _logger = logger;
    }

    public Task<double?> GetRelationshipDepthAsync(string artistId, string relatedArtistId, CancellationToken ct)
    {
        // Stub for first pass; to be implemented behind feature toggle
        _logger.LogDebug("MusicBrainzProvider: stub called for {ArtistId} -> {RelatedId}", artistId, relatedArtistId);
        return Task.FromResult<double?>(null);
    }
}
