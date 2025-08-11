# Lidarr.Recommendations (v0.1.0)

[![CI](https://github.com/zachyzissou/lidarr-recommendation-engine/actions/workflows/ci.yml/badge.svg)](https://github.com/zachyzissou/lidarr-recommendation-engine/actions/workflows/ci.yml)

Recommendations for Lidarr: Similar Artists, Album Gaps, and New & Upcoming releases — offline-first, optional enrichment later.

Highlights

- Offline-only baseline (no API keys needed)
- Native UI pages: Settings and Discover (tabs, sorting, actions)
- Deterministic scoring with explainable “Reason” strings
- Optional stubs for ListenBrainz and MusicBrainz behind toggles

Requirements

- Lidarr “plugins” branch with Plugins page enabled
- .NET 8 SDK
- Windows, Linux, or macOS

Quick start

1. Build and package (Windows/PowerShell):

```powershell
pwsh -File build/package.ps1 -Version 0.1.0
```

1. Install in Lidarr:

- System → Plugins → Install from URL/Zip → select artifacts/Lidarr.Recommendations-v0.1.0.zip
- Configure settings under System → Plugins → Lidarr.Recommendations
- Open Discover → Recommendations

Smoke test (what you should see)

- Discover shows tabs: Similar Artists, Album Gaps, New & Upcoming
- Sorting options: Score, Recency, Popularity, Name
- Each item shows a Score and Reason
- Logs include: “Plugin started”

Architecture

```text
src/Lidarr.Recommendations/
  Plugin.cs                     # Entry/DI
  plugin.json                   # Manifest
  Config/
    PluginSettings.cs          # Toggles, sliders, filters
    SettingKeys.cs
  Domain/
    ArtistProfile.cs, AlbumProfile.cs, Recommendation.cs, Reason.cs, RecommendationType.cs
  Services/
    ILibraryAdapter.cs, LibraryAdapter.cs
    FeatureEngineer.cs         # Vectors + cosine
    RecommendationEngine.cs    # Ranking and result shaping
    ScoreWeights.cs
    Caching/
      IMemoryCacheService.cs, MemoryCacheService.cs
    Providers/
      IRecommendationSignalProvider.cs
      LocalSignalProvider.cs   # Offline related artists
      ListenBrainzProvider.cs, MusicBrainzProvider.cs (stubs)
  UI/
    Settings/ (index.html, settings.js, settings.css)
    Discover/ (discover.html, discover.js, discover.css)
  Resources/i18n/en-US.json
tests/
  Lidarr.Recommendations.Tests/ (xUnit + Moq + FluentAssertions)
build/
  package.ps1                  # Build + zip
.github/workflows/ci.yml       # CI build + test + package
```

Scoring (first pass)

```text
score = w1*Similarity(local_features)
      + w2*CoListen(ListenBrainz?)
      + w3*MB_RelationshipDepth
      + w4*PopularityProxy(if available)
      - w5*AlreadyOwnedPenalty
      - w6*ExcludedTagPenalty
```

Local features include tags/genres, decade, country, label, and co-occurrence proxies.

Build locally

```powershell
dotnet restore src/Lidarr.Recommendations/Lidarr.Recommendations.csproj
dotnet build src/Lidarr.Recommendations/Lidarr.Recommendations.csproj -c Release
dotnet test tests/Lidarr.Recommendations.Tests/Lidarr.Recommendations.Tests.csproj -c Release
```

Package

```powershell
pwsh -File build/package.ps1 -Version 0.1.0
```

Install in Lidarr

- System → Plugins → Install from URL/Zip → pick artifacts/Lidarr.Recommendations-v0.1.0.zip
- Configure settings (toggles and sliders), then open the Discover page

Contributing

- Issues and PRs welcome. Please include a clear scenario and reproduction if reporting bugs.
- Before PR: run build/test locally. Keep changes focused and add tests when public behavior changes.

Roadmap

- SDK alignment: adapt entrypoint and UI routing to latest Lidarr plugin SDK
- LibraryAdapter: integrate real host APIs (artists, albums, history, tags)
- Indexer feed: enable a virtual indexer if third-party support is available
- Enrichment providers: implement ListenBrainz co-listen and MusicBrainz relationship depth
- Caching & resiliency: per-provider TTLs, 429/5xx backoff, and circuit breakers
- Filters: include/exclude by tag/genre/year/country; hide live/compilations; prefer lossless
- Actions: native “Add Artist/Album/Wanted,” MB/LB links, pin/less-like-this
- i18n: add locales beyond en-US
- Telemetry (opt-in): basic usage metrics to guide improvements

License
MIT — see LICENSE.

