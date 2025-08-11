using Lidarr.Recommendations.Domain;
using Microsoft.Extensions.Logging;

namespace Lidarr.Recommendations.Services.Providers;

public sealed class LocalSignalProvider : IRecommendationSignalProvider
{
    private readonly ILibraryAdapter _lib;
    private readonly FeatureEngineer _fe;
    private readonly ILogger<LocalSignalProvider> _logger;

    public LocalSignalProvider(ILibraryAdapter lib, FeatureEngineer fe, ILogger<LocalSignalProvider> logger)
    {
        _lib = lib; _fe = fe; _logger = logger;
    }

    public async Task<IReadOnlyList<(ArtistProfile artist, double similarity)>> GetRelatedArtistsAsync(string artistId, CancellationToken ct)
    {
        var artists = await _lib.GetArtistsAsync(ct);
        var index = artists.ToDictionary(a => a.Id, a => a, StringComparer.OrdinalIgnoreCase);
        if (!index.TryGetValue(artistId, out var seed)) return Array.Empty<(ArtistProfile, double)>();

        var seedVec = _fe.BuildArtistVector(seed);
        var list = new List<(ArtistProfile, double)>();
        foreach (var a in artists)
        {
            if (a.Id == artistId) continue;
            var sim = _fe.Cosine(seedVec, _fe.BuildArtistVector(a));
            if (sim > 0) list.Add((a, sim));
        }
        _logger.LogDebug("LocalSignalProvider: computed {Count} related for {ArtistId}", list.Count, artistId);
        return list.OrderByDescending(x => x.Item2).Take(50).ToList();
    }

    public Task<double?> GetPopularityAsync(string entityId, CancellationToken ct)
    {
        // Popularity proxy: not available offline reliably. Return null.
        return Task.FromResult<double?>(null);
    }
}
