namespace Jogging.Persistence.Models.CompetitionResult
{
    public partial class SimpleCompetitionResult
    {
        public int Id { get; set; }
        public short? RunNumber { get; set; }
        public TimeSpan? RunTime { get; set; }
        public int CompetitionPerCategoryId { get; set; }
        public bool? Paid { get; set; }
        public int CompetitionId { get; set; }
        public int PersonId { get; set; }
    }
}
