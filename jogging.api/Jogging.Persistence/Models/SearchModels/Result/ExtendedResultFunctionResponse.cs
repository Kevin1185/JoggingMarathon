namespace Jogging.Persistence.Models.SearchModels.Result;

public class ExtendedResultFunctionResponse
{
    public string? CompetitionId { get; set; }
    public TimeSpan RunTime { get; set; }
    public int PersonId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public char Gender { get; set; }
    public string? DistanceName { get; set; }
    public string? AgeCategoryName { get; set; }
}