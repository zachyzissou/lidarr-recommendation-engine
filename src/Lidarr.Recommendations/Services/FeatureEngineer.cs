using Lidarr.Recommendations.Domain;

namespace Lidarr.Recommendations.Services;

public sealed class FeatureEngineer
{
	// Convert tags/decades/country/labels to a sparse normalized vector for similarity
	public Dictionary<string, double> BuildArtistVector(ArtistProfile a)
	{
		var v = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

		foreach (var t in a.Tags) v[$"tag:{t}"] = v.GetValueOrDefault($"tag:{t}") + 1.0;
		foreach (var d in a.Decades) v[$"decade:{d}"] = 1.0;
		if (!string.IsNullOrWhiteSpace(a.Country)) v[$"country:{a.Country}"] = 1.0;
		foreach (var l in a.Labels) v[$"label:{l}"] = 1.0;

		// Normalize L2
		var norm = Math.Sqrt(v.Values.Sum(x => x * x));
		if (norm > 0) foreach (var k in v.Keys.ToList()) v[k] = v[k] / norm;

		return v;
	}

	public double Cosine(Dictionary<string, double> a, Dictionary<string, double> b)
	{
		if (a.Count == 0 || b.Count == 0) return 0;
		double sum = 0;
		foreach (var (k, va) in a)
		{
			if (b.TryGetValue(k, out var vb)) sum += va * vb;
		}
		return Math.Clamp(sum, 0, 1);
	}

	public double Jaccard<T>(ICollection<T> a, ICollection<T> b)
	{
		if (a.Count == 0 && b.Count == 0) return 0;
		var setA = a is HashSet<T> ha ? ha : new HashSet<T>(a);
		var setB = b is HashSet<T> hb ? hb : new HashSet<T>(b);
		int inter = setA.Intersect(setB).Count();
		int union = setA.Count + setB.Count - inter;
		return union == 0 ? 0 : (double)inter / union;
	}
}

