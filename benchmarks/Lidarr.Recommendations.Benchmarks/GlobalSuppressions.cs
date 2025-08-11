// Global analyzer suppressions for Lidarr.Recommendations.Benchmarks

using System.Diagnostics.CodeAnalysis;

// CA1707: Remove underscores from member names
// Suppressed for benchmark methods - underscores are conventional in BenchmarkDotNet for clarity
[assembly: SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Benchmark method names conventionally use underscores for clarity")]

// CA2007: Consider calling ConfigureAwait
// Suppressed for benchmarks - ConfigureAwait is not needed in benchmark context
[assembly: SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task", Justification = "ConfigureAwait not required in benchmark context")]
