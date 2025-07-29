namespace Jogging.Persistence.Models;

public class CompetitionResultData
{
    public int CompetitionId { get; set; }
    public List<DistanceResult>? Distances { get; set; }
}
