using System.Reflection;
using Lidarr.Recommendations.Config;
using Lidarr.Recommendations.Services;
using Lidarr.Recommendations.Services.Caching;
using Lidarr.Recommendations.Services.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

// NOTE: This entry type shape follows a pragmatic IPlugin-style contract.
// Lidarr plugins branch may expose a different entrypoint signature. If so,
// adapt this class to the actual SDK (see Decisions & TODOs).

namespace Lidarr.Recommendations;

public sealed class Plugin /* : IPlugin (adapt to real SDK) */
{
    private ILogger? _logger;
    private IServiceProvider? _provider;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1822:Mark members as static", Justification = "Plugin contract requires instance members")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2325:Methods and properties that don't access instance data should be static", Justification = "Plugin contract requires instance members")]
    public string Id => "lidarr.recommendations";

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1822:Mark members as static", Justification = "Plugin contract requires instance members")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2325:Methods and properties that don't access instance data should be static", Justification = "Plugin contract requires instance members")]
    public string Name => "Lidarr.Recommendations";

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1822:Mark members as static", Justification = "Plugin contract requires instance members")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2325:Methods and properties that don't access instance data should be static", Justification = "Plugin contract requires instance members")]
    public Version Version => Assembly.GetExecutingAssembly().GetName().Version ?? new Version(0, 1, 0, 0);

    // Called by host to register services
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2325:Methods and properties that don't access instance data should be static", Justification = "Plugin contract requires instance methods")]
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddOptions<PluginSettings>();
        services.AddSingleton<IMemoryCacheService, MemoryCacheService>();
        services.AddSingleton<ILibraryAdapter, LibraryAdapter>();
        services.AddSingleton<RecommendationEngine>();
        services.AddSingleton<IRecommendationSignalProvider, LocalSignalProvider>();
        services.AddSingleton<ListenBrainzProvider>();
        services.AddSingleton<MusicBrainzProvider>();
        services.AddLogging();
    }

    // Called by host to start plugin
    public void Start(IServiceProvider provider, ILogger logger, IOptions<PluginSettings> settings)
    {
        _provider = provider;
        _logger = logger;
        _logger.LogInformation("[Lidarr.Recommendations] Plugin started v{Version}", Version);

        // Warm up cache lightly
        _ = Task.Run(async () =>
        {
            try
            {
                var engine = _provider.GetRequiredService<RecommendationEngine>();
                await engine.PrimeAsync(settings.Value, CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Warm-up failed (non-fatal).");
            }
        });
    }

    public void Stop()
    {
        _logger?.LogInformation("[Lidarr.Recommendations] Plugin stopped");
    }

    // Optional lightweight API methods the host can map (adapt routes to SDK)
    public async Task<IReadOnlyList<Domain.Recommendation>> GetRecommendationsAsync(
        string kind, // "similar" | "gaps" | "upcoming"
        int? take,
        CancellationToken ct)
    {
        var engine = _provider!.GetRequiredService<RecommendationEngine>();
        return kind?.ToUpperInvariant() switch
        {
            "SIMILAR" => await engine.GetSimilarArtistsAsync(take ?? 50, ct).ConfigureAwait(false),
            "GAPS" => await engine.GetAlbumGapsAsync(take ?? 50, ct).ConfigureAwait(false),
            "upcoming" => await engine.GetNewAndUpcomingAsync(take ?? 50, ct).ConfigureAwait(false),
            _ => Array.Empty<Domain.Recommendation>()
        };
    }
}
