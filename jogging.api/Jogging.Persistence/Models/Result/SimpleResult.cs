namespace Jogging.Persistence.Models.Result;
public class SimpleResult
{
    public int Id { get; set; }
    public short? RunNumber { get; set; }
    public TimeSpan? RunTime { get; set; }
    public int CompetitionPerCategoryId { get; set; }
    public bool? Paid { get; set; }
    public int CompetitionId { get; set; }
    public int PersonId { get; set; }
}