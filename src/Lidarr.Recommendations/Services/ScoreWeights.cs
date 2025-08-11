namespace Lidarr.Recommendations.Config;

public sealed class ScoreWeights
{
    public double Similarity { get; set; } = 0.4;   // w1
    public double CoListen { get; set; } = 0.2;     // w2
    public double MbDepth { get; set; } = 0.2;      // w3
    public double Popularity { get; set; } = 0.1;   // w4
    public double OwnedPenalty { get; set; } = 0.2; // w5
    public double ExcludedTagPenalty { get; set; } = 0.2; // w6
}
