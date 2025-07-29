namespace Jogging.Persistence.Models;

public class DistanceResult
{
    public string? DistanceName { get; set; }
    public DateTime? GunTime { get; set; }
    public string? AgeCategory { get; set; }
    public List<ParticipantResult>? Participants { get; set; }
}
