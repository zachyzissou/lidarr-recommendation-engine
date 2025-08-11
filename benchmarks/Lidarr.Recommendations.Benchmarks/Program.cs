using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Lidarr.Recommendations.Services;
using Lidarr.Recommendations.Services.Providers;
using Lidarr.Recommendations.Tests;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Lidarr.Recommendations.Benchmarks;

[MemoryDiagnoser]
[SimpleJob]
public class RecommendationPipelineBenchmarks
{
    private List<Domain.ArtistProfile> _artists100 = null!;
    private List<Domain.ArtistProfile> _artists1k = null!;
    private List<Domain.ArtistProfile> _artists10k = null!;
    private LocalSignalProvider _provider = null!;

    [GlobalSetup]
    public void Setup()
    {
        _artists100 = TestData.CreateLargeTestLibrary(100);
        _artists1k = TestData.CreateLargeTestLibrary(1000);
        _artists10k = TestData.CreateLargeTestLibrary(10000);

        var lib = new Mock<ILibraryAdapter>();
        _provider = new LocalSignalProvider(lib.Object, NullLogger<LocalSignalProvider>.Instance);
    }

    [Benchmark]
    [Arguments(100)]
    [Arguments(1000)]
    [Arguments(10000)]
    public double FeatureEngineer_BuildArtistVector(int artistCount)
    {
        var artists = artistCount switch
        {
            100 => _artists100,
            1000 => _artists1k,
            10000 => _artists10k,
            _ => throw new ArgumentException("Invalid artist count")
        };

        double totalNorm = 0;
        foreach (var artist in artists)
        {
            var vector = FeatureEngineer.BuildArtistVector(artist);
            totalNorm += vector.Values.Sum(v => v * v);
        }
        return totalNorm;
    }

    [Benchmark]
    [Arguments(100)]
    [Arguments(1000)]
    [Arguments(10000)]
    public double FeatureEngineer_CosineSimilarity(int artistCount)
    {
        var artists = artistCount switch
        {
            100 => _artists100,
            1000 => _artists1k,
            10000 => _artists10k,
            _ => throw new ArgumentException("Invalid artist count")
        };

        var vectors = artists.Select(FeatureEngineer.BuildArtistVector).ToList();
        double totalSimilarity = 0;

        for (int i = 0; i < Math.Min(50, vectors.Count); i++)
        {
            for (int j = i + 1; j < Math.Min(50, vectors.Count); j++)
            {
                totalSimilarity += FeatureEngineer.Cosine(vectors[i], vectors[j]);
            }
        }

        return totalSimilarity;
    }

    [Benchmark]
    [Arguments(100)]
    [Arguments(1000)]
    public async Task<int> LocalSignalProvider_GetRelatedArtists(int artistCount)
    {
        var artists = artistCount switch
        {
            100 => _artists100,
            1000 => _artists1k,
            _ => throw new ArgumentException("Invalid artist count")
        };

        var lib = new Mock<ILibraryAdapter>();
        lib.Setup(x => x.GetArtistsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(artists);

        var provider = new LocalSignalProvider(lib.Object, NullLogger<LocalSignalProvider>.Instance);
        var seedArtist = artists.First();

        var results = await provider.GetRelatedArtistsAsync(seedArtist.Id, CancellationToken.None).ConfigureAwait(false);
        return results.Count;
    }

    [Benchmark]
    [Arguments(100)]
    [Arguments(1000)]
    public async Task<int> RecommendationEngine_GetSimilarArtists(int artistCount)
    {
        var artists = artistCount switch
        {
            100 => _artists100,
            1000 => _artists1k,
            _ => throw new ArgumentException("Invalid artist count")
        };

        var lib = new Mock<ILibraryAdapter>();
        lib.Setup(x => x.GetArtistsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(artists);
        lib.Setup(x => x.GetOwnedArtistIdsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new HashSet<string>());

        var localProvider = new LocalSignalProvider(lib.Object, NullLogger<LocalSignalProvider>.Instance);
        var engine = new RecommendationEngine(
            lib.Object,
            localProvider,
            new ListenBrainzProvider(NullLogger<ListenBrainzProvider>.Instance),
            new MusicBrainzProvider(NullLogger<MusicBrainzProvider>.Instance),
            NullLogger<RecommendationEngine>.Instance
        );

        var results = await engine.GetSimilarArtistsAsync(50, CancellationToken.None).ConfigureAwait(false);
        return results.Count;
    }
}

public static class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<RecommendationPipelineBenchmarks>();
    }
}
