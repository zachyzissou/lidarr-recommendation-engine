using System.Diagnostics.CodeAnalysis;
using Lidarr.Recommendations.Domain;

namespace Lidarr.Recommendations.Services;

public static class FeatureEngineer
{
    // Convert tags/decades/country/labels to a sparse normalized vector for similarity
    public static Dictionary<string, double> BuildArtistVector([NotNull] ArtistProfile artist)
    {
        ArgumentNullException.ThrowIfNull(artist);

        var vector = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

        foreach (var tag in artist.Tags)
        {
            vector[$"tag:{tag}"] = vector.GetValueOrDefault($"tag:{tag}") + 1.0;
        }
        foreach (var decade in artist.Decades)
        {
            vector[$"decade:{decade}"] = 1.0;
        }
        if (!string.IsNullOrWhiteSpace(artist.Country))
        {
            vector[$"country:{artist.Country}"] = 1.0;
        }
        foreach (var label in artist.Labels)
        {
            vector[$"label:{label}"] = 1.0;
        }

        // Normalize L2
        var norm = Math.Sqrt(vector.Values.Sum(x => x * x));
        if (norm > 0)
        {
            foreach (var key in vector.Keys.ToList())
            {
                vector[key] = vector[key] / norm;
            }
        }

        return vector;
    }

    public static double Cosine([NotNull] Dictionary<string, double> vectorA, [NotNull] Dictionary<string, double> vectorB)
    {
        ArgumentNullException.ThrowIfNull(vectorA);
        ArgumentNullException.ThrowIfNull(vectorB);

        if (vectorA.Count == 0 || vectorB.Count == 0)
        {
            return 0;
        }

        double sum = 0;
        foreach (var (key, valueA) in vectorA)
        {
            if (vectorB.TryGetValue(key, out var valueB))
            {
                sum += valueA * valueB;
            }
        }
        return Math.Clamp(sum, 0, 1);
    }

    public static double Jaccard<T>([NotNull] ICollection<T> collectionA, [NotNull] ICollection<T> collectionB)
    {
        ArgumentNullException.ThrowIfNull(collectionA);
        ArgumentNullException.ThrowIfNull(collectionB);

        if (collectionA.Count == 0 && collectionB.Count == 0)
        {
            return 0;
        }

        var setA = collectionA is HashSet<T> hashSetA ? hashSetA : new HashSet<T>(collectionA);
        var setB = collectionB is HashSet<T> hashSetB ? hashSetB : new HashSet<T>(collectionB);
        int intersection = setA.Intersect(setB).Count();
        int union = setA.Count + setB.Count - intersection;
        return union == 0 ? 0 : (double)intersection / union;
    }
}

