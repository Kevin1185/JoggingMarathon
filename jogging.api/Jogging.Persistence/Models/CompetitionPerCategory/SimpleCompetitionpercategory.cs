namespace Jogging.Persistence.Models.CompetitionPerCategory;

public partial class SimpleCompetitionpercategory
{
    public int Id { get; set; }

    public string? Gender { get; set; }

    public string? DistanceName { get; set; }

    public float? DistanceInKm { get; set; }

    public int? AgeCategoryId { get; set; }

    public int? CompetitionId { get; set; }

    public DateTime? GunTime { get; set; }
}
