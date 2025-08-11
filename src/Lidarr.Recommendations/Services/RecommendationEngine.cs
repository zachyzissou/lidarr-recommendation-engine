using Lidarr.Recommendations.Config;
using Lidarr.Recommendations.Domain;
using Lidarr.Recommendations.Services.Providers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Lidarr.Recommendations.Services;

public sealed class RecommendationEngine
{
    private readonly ILibraryAdapter _lib;
    private readonly IRecommendationSignalProvider _local;
    private readonly ListenBrainzProvider _lb;
    private readonly MusicBrainzProvider _mb;
    private readonly FeatureEngineer _fe;
    private readonly ILogger<RecommendationEngine> _logger;

    public RecommendationEngine(
        ILibraryAdapter lib,
        IRecommendationSignalProvider local,
        ListenBrainzProvider lb,
        MusicBrainzProvider mb,
        FeatureEngineer fe,
        ILogger<RecommendationEngine> logger)
    {
        _lib = lib; _local = local; _lb = lb; _mb = mb; _fe = fe; _logger = logger;
    }

    public Task PrimeAsync(PluginSettings settings, CancellationToken ct)
    {
        // No-op for now; placeholder for cache prime
        _logger.LogInformation("RecommendationEngine: prime called (OfflineOnly={Offline})", settings.OfflineOnly);
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<Recommendation>> GetSimilarArtistsAsync(int take, CancellationToken ct)
    {
        var artists = await _lib.GetArtistsAsync(ct);
        var owned = await _lib.GetOwnedArtistIdsAsync(ct);

        var recs = new List<Recommendation>();
        foreach (var seed in artists)
        {
            var related = await _local.GetRelatedArtistsAsync(seed.Id, ct);
            foreach (var (artist, sim) in related)
            {
                if (owned.Contains(artist.Id)) continue;
                var reason = new Reason { Summary = $"Similar to {seed.Name}; tags overlap" };
                var score = sim; // baseline similarity
                recs.Add(new Recommendation
                {
                    Id = artist.Id,
                    Title = artist.Name,
                    Type = RecommendationType.SimilarArtist,
                    Score = score,
                    Reason = reason,
                    SuggestedActions = new[] { "Add Artist", "View on MB" }
                });
            }
        }

        return recs
            .OrderByDescending(r => r.Score)
            .GroupBy(r => r.Id)
            .Select(g => g.First())
            .Take(take)
            .ToList();
    }

    public async Task<IReadOnlyList<Recommendation>> GetAlbumGapsAsync(int take, CancellationToken ct)
    {
        var artists = await _lib.GetArtistsAsync(ct);
        var recs = new List<Recommendation>();
        foreach (var a in artists)
        {
            var albums = await _lib.GetAlbumsByArtistAsync(a.Id, ct);
            var owned = albums.Where(x => x.IsOwned).Select(x => x.Id).ToHashSet();
            foreach (var album in albums)
            {
                if (album.IsOwned) continue;
                if (album.IsLive || album.IsCompilation) continue;
                recs.Add(new Recommendation
                {
                    Id = album.Id,
                    ParentId = a.Id,
                    Title = album.Title,
                    Subtitle = a.Name,
                    Type = RecommendationType.AlbumGap,
                    Score = 0.5,
                    Reason = new Reason { Summary = $"Missing album by {a.Name}" },
                    SuggestedActions = new[] { "Add Album", "Add to Wanted" }
                });
            }
        }
        return recs.OrderByDescending(r => r.Score).Take(take).ToList();
    }

    public async Task<IReadOnlyList<Recommendation>> GetNewAndUpcomingAsync(int take, CancellationToken ct)
    {
        var artists = await _lib.GetArtistsAsync(ct);
        var recs = new List<Recommendation>();
        foreach (var a in artists)
        {
            var albums = await _lib.GetAlbumsByArtistAsync(a.Id, ct);
            foreach (var album in albums)
            {
                if (!album.IsUpcoming) continue;
                recs.Add(new Recommendation
                {
                    Id = album.Id,
                    ParentId = a.Id,
                    Title = album.Title,
                    Subtitle = a.Name,
                    Type = RecommendationType.NewOrUpcoming,
                    Score = 0.6,
                    Reason = new Reason { Summary = $"Upcoming release for {a.Name}" },
                    SuggestedActions = new[] { "Add to Wanted", "View on MB" }
                });
            }
        }
        return recs.OrderByDescending(r => r.Score).Take(take).ToList();
    }
}
