namespace Lidarr.Recommendations.Domain;

public sealed class Reason
{
    public string Summary { get; set; } = "";
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Domain object requires mutable collections for serialization")]
    public Dictionary<string, string> Details { get; set; } = new();

    public override string ToString() => Summary;
}
