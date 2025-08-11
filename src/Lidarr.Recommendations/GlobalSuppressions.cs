// Global analyzer suppressions for Lidarr.Recommendations plugin

using System.Diagnostics.CodeAnalysis;

// CA1848: Use LoggerMessage delegates for better performance
// Suppressed for plugin context - the performance impact is minimal for plugin operations
// and the added complexity of LoggerMessage delegates is not justified
[assembly: SuppressMessage("Performance", "CA1848:Use the LoggerMessage delegates instead of calling LoggerExtensions methods", Justification = "Plugin operations are infrequent; LoggerMessage delegate complexity not justified")]
