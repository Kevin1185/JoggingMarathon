using Jogging.Persistence.Models.AgeCategory;


namespace Jogging.Persistence.Models.CompetitionPerCategory;
public partial class CompetitionResultCompetitionPerCategory
{
    public int Id { get; set; }

    public string? DistanceName { get; set; }

    public float DistanceInKm { get; set; }
  
    public char Gender { get; set; }

    public int AgeCategoryId { get; set; }

    public int CompetitionId { get; set; }

    public DateTime? GunTime { get; set; }

    public SimpleAgeCategory? AgeCategory { get; set; }
}
