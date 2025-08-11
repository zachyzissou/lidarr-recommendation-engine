using Lidarr.Recommendations.Domain;

namespace Lidarr.Recommendations.Tests;

/// <summary>
/// Test data factory for creating consistent test objects
/// </summary>
public static class TestData
{
    public static ArtistProfile CreateArtist(
        string id,
        string? name = null,
        IEnumerable<string>? tags = null,
        IEnumerable<int>? decades = null,
        IEnumerable<string>? labels = null,
        string? country = null,
        double popularity = 0.0)
    {
        return new ArtistProfile
        {
            Id = id,
            Name = name ?? id,
            Tags = new HashSet<string>(tags ?? [], StringComparer.OrdinalIgnoreCase),
            Decades = new HashSet<int>(decades ?? []),
            Labels = new HashSet<string>(labels ?? [], StringComparer.OrdinalIgnoreCase),
            Country = country,
            PopularityProxy = popularity,
            RelatedArtistIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        };
    }

    public static AlbumProfile CreateAlbum(
        string id,
        string artistId,
        string? title = null,
        bool isOwned = false,
        bool isLive = false,
        bool isCompilation = false,
        bool isUpcoming = false,
        int? year = null)
    {
        return new AlbumProfile
        {
            Id = id,
            ArtistId = artistId,
            Title = title ?? id,
            IsOwned = isOwned,
            IsLive = isLive,
            IsCompilation = isCompilation,
            IsUpcoming = isUpcoming,
            Year = year
        };
    }

    /// <summary>
    /// Creates a small test library with known relationships
    /// </summary>
    public static List<ArtistProfile> CreateSmallTestLibrary()
    {
        return new List<ArtistProfile>
        {
            CreateArtist("artist-1", "The Beatles", ["rock", "pop", "60s"], [1960], ["Apple Records"], "UK", 0.9),
            CreateArtist("artist-2", "Led Zeppelin", ["rock", "hard rock", "70s"], [1970], ["Atlantic"], "UK", 0.85),
            CreateArtist("artist-3", "Pink Floyd", ["rock", "progressive", "70s"], [1970], ["EMI"], "UK", 0.8),
            CreateArtist("artist-4", "Miles Davis", ["jazz", "fusion"], [1950, 1960], ["Columbia"], "US", 0.75),
            CreateArtist("artist-5", "John Coltrane", ["jazz", "bebop"], [1950, 1960], ["Blue Note"], "US", 0.7)
        };
    }

    /// <summary>
    /// Creates a larger test library for performance testing
    /// </summary>
    public static List<ArtistProfile> CreateLargeTestLibrary(int size)
    {
        var random = new Random(42); // Seeded for determinism
        var genres = new[] { "rock", "pop", "jazz", "metal", "electronic", "folk", "country", "hip-hop", "blues", "classical" };
        var countries = new[] { "US", "UK", "Canada", "Germany", "France", "Japan", "Australia" };
        var decades = new[] { 1960, 1970, 1980, 1990, 2000, 2010, 2020 };

        var artists = new List<ArtistProfile>();
        for (int i = 0; i < size; i++)
        {
            var genreCount = random.Next(1, 4);
            var selectedGenres = genres.OrderBy(_ => random.Next()).Take(genreCount);
            var selectedDecades = decades.OrderBy(_ => random.Next()).Take(random.Next(1, 3));
            var country = countries[random.Next(countries.Length)];

            artists.Add(CreateArtist(
                $"artist-{i:D6}",
                $"Artist {i}",
                selectedGenres,
                selectedDecades,
                [$"Label {random.Next(1, 100)}"],
                country,
                random.NextDouble()
            ));
        }

        return artists;
    }
}