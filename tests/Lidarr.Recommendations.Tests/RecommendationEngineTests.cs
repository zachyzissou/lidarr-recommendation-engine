using FluentAssertions;
using Lidarr.Recommendations.Config;
using Lidarr.Recommendations.Domain;
using Lidarr.Recommendations.Services;
using Lidarr.Recommendations.Services.Providers;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Lidarr.Recommendations.Tests;

public class RecommendationEngineTests
{
    [Fact]
    public async Task SimilarArtists_should_not_throw_with_empty_library()
    {
        var lib = new Mock<ILibraryAdapter>();
        lib.Setup(x => x.GetArtistsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<ArtistProfile>());
        lib.Setup(x => x.GetOwnedArtistIdsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new HashSet<string>());

        var local = new LocalSignalProvider(lib.Object, new FeatureEngineer(), NullLogger<LocalSignalProvider>.Instance);
        var engine = new RecommendationEngine(lib.Object, local, new ListenBrainzProvider(NullLogger<ListenBrainzProvider>.Instance), new MusicBrainzProvider(NullLogger<MusicBrainzProvider>.Instance), new FeatureEngineer(), NullLogger<RecommendationEngine>.Instance);

        var results = await engine.GetSimilarArtistsAsync(10, CancellationToken.None);
        results.Should().NotBeNull();
        results.Should().BeEmpty();
    }
}
