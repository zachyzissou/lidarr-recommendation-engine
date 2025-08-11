using Lidarr.Recommendations.Domain;

namespace Lidarr.Recommendations.Services.Providers;

public interface IRecommendationSignalProvider
{
    Task<IReadOnlyList<(ArtistProfile artist, double similarity)>> GetRelatedArtistsAsync(string artistId, CancellationToken ct);
    Task<double?> GetPopularityAsync(string entityId, CancellationToken ct);
}
