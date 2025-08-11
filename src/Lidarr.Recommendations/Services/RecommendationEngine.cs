using Lidarr.Recommendations.Config;
using Lidarr.Recommendations.Domain;
using Lidarr.Recommendations.Services.Providers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Lidarr.Recommendations.Services;

public sealed class RecommendationEngine
{
    private readonly ILibraryAdapter _library;
    private readonly IRecommendationSignalProvider _localProvider;
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "S4487:Unread private fields should be removed", Justification = "Will be used when external provider integrations are implemented")]
    private readonly ListenBrainzProvider _listenBrainzProvider;
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "S4487:Unread private fields should be removed", Justification = "Will be used when external provider integrations are implemented")]
    private readonly MusicBrainzProvider _musicBrainzProvider;
    private readonly ILogger<RecommendationEngine> _logger;

    public RecommendationEngine(
        ILibraryAdapter library,
        IRecommendationSignalProvider localProvider,
        ListenBrainzProvider listenBrainzProvider,
        MusicBrainzProvider musicBrainzProvider,
        ILogger<RecommendationEngine> logger)
    {
        _library = library;
        _localProvider = localProvider;
        _listenBrainzProvider = listenBrainzProvider;
        _musicBrainzProvider = musicBrainzProvider;
        _logger = logger;
    }

    public Task PrimeAsync(PluginSettings settings, CancellationToken cancellationToken)
    {
        // No-op for now; placeholder for cache prime
        _logger.LogInformation("RecommendationEngine: prime called (OfflineOnly={Offline})", settings.OfflineOnly);
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<Recommendation>> GetSimilarArtistsAsync(int take, CancellationToken cancellationToken)
    {
        var artists = await _library.GetArtistsAsync(cancellationToken).ConfigureAwait(false);
        var owned = await _library.GetOwnedArtistIdsAsync(cancellationToken).ConfigureAwait(false);

        var recommendations = new List<Recommendation>();
        foreach (var seedArtist in artists)
        {
            var relatedArtists = await _localProvider.GetRelatedArtistsAsync(seedArtist.Id, cancellationToken).ConfigureAwait(false);
            foreach (var (artist, similarity) in relatedArtists)
            {
                if (owned.Contains(artist.Id))
                {
                    continue;
                }

                var reason = new Reason { Summary = $"Similar to {seedArtist.Name}; tags overlap" };
                var score = similarity; // baseline similarity
                recommendations.Add(new Recommendation
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

        return recommendations
            .OrderByDescending(r => r.Score)
            .GroupBy(r => r.Id)
            .Select(g => g.First())
            .Take(take)
            .ToList();
    }

    public async Task<IReadOnlyList<Recommendation>> GetAlbumGapsAsync(int take, CancellationToken cancellationToken)
    {
        var artists = await _library.GetArtistsAsync(cancellationToken).ConfigureAwait(false);
        var recommendations = new List<Recommendation>();

        foreach (var artist in artists)
        {
            var albums = await _library.GetAlbumsByArtistAsync(artist.Id, cancellationToken).ConfigureAwait(false);

            foreach (var album in albums)
            {
                if (album.IsOwned || album.IsLive || album.IsCompilation)
                {
                    continue;
                }

                recommendations.Add(new Recommendation
                {
                    Id = album.Id,
                    ParentId = artist.Id,
                    Title = album.Title,
                    Subtitle = artist.Name,
                    Type = RecommendationType.AlbumGap,
                    Score = 0.5,
                    Reason = new Reason { Summary = $"Missing album by {artist.Name}" },
                    SuggestedActions = new[] { "Add Album", "Add to Wanted" }
                });
            }
        }
        return recommendations.OrderByDescending(r => r.Score).Take(take).ToList();
    }

    public async Task<IReadOnlyList<Recommendation>> GetNewAndUpcomingAsync(int take, CancellationToken cancellationToken)
    {
        var artists = await _library.GetArtistsAsync(cancellationToken).ConfigureAwait(false);
        var recommendations = new List<Recommendation>();

        foreach (var artist in artists)
        {
            var albums = await _library.GetAlbumsByArtistAsync(artist.Id, cancellationToken).ConfigureAwait(false);
            foreach (var album in albums)
            {
                if (!album.IsUpcoming)
                {
                    continue;
                }

                recommendations.Add(new Recommendation
                {
                    Id = album.Id,
                    ParentId = artist.Id,
                    Title = album.Title,
                    Subtitle = artist.Name,
                    Type = RecommendationType.NewOrUpcoming,
                    Score = 0.6,
                    Reason = new Reason { Summary = $"Upcoming release for {artist.Name}" },
                    SuggestedActions = new[] { "Add to Wanted", "View on MB" }
                });
            }
        }
        return recommendations.OrderByDescending(r => r.Score).Take(take).ToList();
    }
}
