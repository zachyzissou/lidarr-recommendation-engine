using FluentAssertions;
using Lidarr.Recommendations.Services;
using Xunit;

namespace Lidarr.Recommendations.Tests.Unit;

/// <summary>
/// Unit tests for FeatureEngineer scoring mathematics, boundaries, and determinism
/// </summary>
public class FeatureEngineerTests
{
    [Fact]
    public void BuildArtistVector_WithNullArtist_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => FeatureEngineer.BuildArtistVector(null!));
    }

    [Fact]
    public void BuildArtistVector_WithEmptyArtist_ReturnsEmptyVector()
    {
        // Arrange
        var artist = TestData.CreateArtist("test", tags: [], decades: [], labels: []);

        // Act
        var vector = FeatureEngineer.BuildArtistVector(artist);

        // Assert
        vector.Should().BeEmpty();
    }

    [Fact]
    public void BuildArtistVector_WithTags_CreatesNormalizedVector()
    {
        // Arrange
        var artist = TestData.CreateArtist("test", tags: ["rock", "metal"]);

        // Act
        var vector = FeatureEngineer.BuildArtistVector(artist);

        // Assert
        vector.Should().HaveCount(2);
        vector.Should().ContainKey("tag:rock");
        vector.Should().ContainKey("tag:metal");
        
        // Verify L2 normalization: sum of squares should be 1
        var sumOfSquares = vector.Values.Sum(v => v * v);
        sumOfSquares.Should().BeApproximately(1.0, 1e-10);
    }

    [Fact]
    public void BuildArtistVector_WithDuplicateTags_AggregatesCorrectly()
    {
        // Arrange
        var artist = TestData.CreateArtist("test", tags: ["rock", "rock", "metal"]);

        // Act
        var vector = FeatureEngineer.BuildArtistVector(artist);

        // Assert - Since tags is a HashSet, duplicates are removed, so both should have equal weight
        vector.Should().HaveCount(2);
        vector["tag:rock"].Should().BeApproximately(vector["tag:metal"], 1e-10);
    }

    [Theory]
    [InlineData(new string[0], new string[0], 0.0)]
    [InlineData(new[] { "rock" }, new string[0], 0.0)]
    [InlineData(new[] { "rock" }, new[] { "metal" }, 0.0)]
    [InlineData(new[] { "rock" }, new[] { "rock" }, 1.0)]
    [InlineData(new[] { "rock", "metal" }, new[] { "rock" }, 0.7071067811865475)] // 1/sqrt(2)
    public void Cosine_WithVariousInputs_ReturnsExpectedSimilarity(string[] tagsA, string[] tagsB, double expected)
    {
        // Arrange
        var artistA = TestData.CreateArtist("A", tags: tagsA);
        var artistB = TestData.CreateArtist("B", tags: tagsB);
        var vectorA = FeatureEngineer.BuildArtistVector(artistA);
        var vectorB = FeatureEngineer.BuildArtistVector(artistB);

        // Act
        var similarity = FeatureEngineer.Cosine(vectorA, vectorB);

        // Assert
        similarity.Should().BeApproximately(expected, 1e-10);
    }

    [Fact]
    public void Cosine_WithNullVectors_ThrowsArgumentNullException()
    {
        // Arrange
        var vector = new Dictionary<string, double>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => FeatureEngineer.Cosine(null!, vector));
        Assert.Throws<ArgumentNullException>(() => FeatureEngineer.Cosine(vector, null!));
    }

    [Fact]
    public void Cosine_IsDeterministic()
    {
        // Arrange
        var artistA = TestData.CreateArtist("A", tags: ["rock", "metal", "progressive"]);
        var artistB = TestData.CreateArtist("B", tags: ["rock", "jazz"]);
        var vectorA = FeatureEngineer.BuildArtistVector(artistA);
        var vectorB = FeatureEngineer.BuildArtistVector(artistB);

        // Act - Run multiple times
        var results = Enumerable.Range(0, 100)
            .Select(_ => FeatureEngineer.Cosine(vectorA, vectorB))
            .ToList();

        // Assert - All results should be identical
        results.Should().AllBeEquivalentTo(results.First());
    }

    [Theory]
    [InlineData(new[] { "rock" }, new[] { "rock" }, 1.0)]
    [InlineData(new[] { "rock", "metal" }, new[] { "rock" }, 0.5)]
    [InlineData(new[] { "rock", "metal" }, new[] { "jazz", "blues" }, 0.0)]
    [InlineData(new string[0], new string[0], 0.0)]
    public void Jaccard_WithVariousInputs_ReturnsExpectedSimilarity(string[] setA, string[] setB, double expected)
    {
        // Act
        var similarity = FeatureEngineer.Jaccard(setA, setB);

        // Assert
        similarity.Should().BeApproximately(expected, 1e-10);
    }

    [Fact]
    public void Jaccard_WithNullCollections_ThrowsArgumentNullException()
    {
        // Arrange
        var collection = new List<string>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => FeatureEngineer.Jaccard<string>(null!, collection));
        Assert.Throws<ArgumentNullException>(() => FeatureEngineer.Jaccard(collection, null!));
    }
}