using Lidarr.Recommendations.Domain;
using Microsoft.Extensions.Logging;

namespace Lidarr.Recommendations.Services.Providers;

public sealed class LocalSignalProvider : IRecommendationSignalProvider
{
    private readonly ILibraryAdapter _library;
    private readonly ILogger<LocalSignalProvider> _logger;

    public LocalSignalProvider(ILibraryAdapter library, FeatureEngineer featureEngineer, ILogger<LocalSignalProvider> logger)
    {
        _library = library; 
        _logger = logger;
        // Note: FeatureEngineer is now static, so we don't need to store it
    }

    public async Task<IReadOnlyList<(ArtistProfile artist, double similarity)>> GetRelatedArtistsAsync(string artistId, CancellationToken cancellationToken)
    {
        var artists = await _library.GetArtistsAsync(cancellationToken).ConfigureAwait(false);
        var index = artists.ToDictionary(a => a.Id, a => a, StringComparer.OrdinalIgnoreCase);
        if (!index.TryGetValue(artistId, out var seedArtist)) 
        {
            return Array.Empty<(ArtistProfile, double)>();
        }

        var seedVector = FeatureEngineer.BuildArtistVector(seedArtist);
        var similarArtists = new List<(ArtistProfile, double)>();
        
        foreach (var artist in artists)
        {
            if (string.Equals(artist.Id, artistId, StringComparison.OrdinalIgnoreCase)) 
            {
                continue;
            }
            
            var similarity = FeatureEngineer.Cosine(seedVector, FeatureEngineer.BuildArtistVector(artist));
            if (similarity > 0) 
            {
                similarArtists.Add((artist, similarity));
            }
        }
        
        _logger.LogDebug("LocalSignalProvider: computed {Count} related artists for {ArtistId}", similarArtists.Count, artistId);
        return similarArtists.OrderByDescending(x => x.Item2).Take(50).ToList();
    }

    public Task<double?> GetPopularityAsync(string entityId, CancellationToken cancellationToken)
    {
        // Popularity proxy: not available offline reliably. Return null.
        return Task.FromResult<double?>(null);
    }
}
