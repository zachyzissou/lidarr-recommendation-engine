// Global analyzer suppressions for Lidarr.Recommendations.Tests

using System.Diagnostics.CodeAnalysis;

// CA1707: Remove underscores from member names
// Suppressed for test methods - underscores improve readability in test names
[assembly: SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Test method names with underscores improve readability")]

// CA1861: Prefer static readonly fields for constant arrays  
// Suppressed for test methods - inline arrays are clearer for test data
[assembly: SuppressMessage("Performance", "CA1861:Avoid constant arrays as arguments", Justification = "Inline test data arrays are more readable than static fields")]

// CA1002: Change List<T> to Collection<T>
// Suppressed for test utilities - List<T> is appropriate for test data creation
[assembly: SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "Test utilities appropriately use List<T> for flexibility")]

// CA2007: Consider calling ConfigureAwait
// Suppressed for test methods - ConfigureAwait is not needed in test context
[assembly: SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task", Justification = "ConfigureAwait not required in test context")]

// CA5394: Random is insecure
// Suppressed for test data generation - cryptographic security not required for tests
[assembly: SuppressMessage("Security", "CA5394:Do not use insecure randomness", Justification = "Non-cryptographic random acceptable for test data generation")]

// xUnit1030: Don't use ConfigureAwait(false) in test methods
// Suppressed since we use ConfigureAwait(false) for consistency but xUnit warns against it
[assembly: SuppressMessage("xUnit", "xUnit1030:Do not call ConfigureAwait(false) in test methods", Justification = "ConfigureAwait(false) used for consistency with library code patterns")]
