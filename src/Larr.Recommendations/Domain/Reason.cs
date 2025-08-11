namespace Lidarr.Recommendations.Domain;

public sealed class Reason
{
    public string Summary { get; set; } = "";
    public Dictionary<string, string> Details { get; set; } = new();

    public override string ToString()
        => Summary;
}
