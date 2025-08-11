using FluentAssertions;
using Lidarr.Recommendations.Domain;
using Lidarr.Recommendations.Services;
using Lidarr.Recommendations.Services.Providers;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Lidarr.Recommendations.Tests;

public class LocalSignalProviderTests
{
    [Fact]
    public async Task RelatedArtists_should_rank_by_cosine()
    {
        var lib = new Mock<ILibraryAdapter>();
        var artists = new List<ArtistProfile>
        {
            new() { Id = "A", Name = "A", Tags = new(StringComparer.OrdinalIgnoreCase){"jazz","hip hop"}},
            new() { Id = "B", Name = "B", Tags = new(StringComparer.OrdinalIgnoreCase){"jazz"}},
            new() { Id = "C", Name = "C", Tags = new(StringComparer.OrdinalIgnoreCase){"metal"}}
        };
        lib.Setup(x => x.GetArtistsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(artists);

        var provider = new LocalSignalProvider(lib.Object, new FeatureEngineer(), NullLogger<LocalSignalProvider>.Instance);
        var rel = await provider.GetRelatedArtistsAsync("A", CancellationToken.None);

        rel.Should().HaveCount(2);
        rel[0].artist.Id.Should().Be("B"); // closer by tags
        rel[1].artist.Id.Should().Be("C");
    }
}
