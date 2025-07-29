namespace Jogging.Persistence.Models
{
    public class PersonData
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Gender { get; set; }
        public DateOnly BirthDate { get; set; }
        public List<BestRunTimeDto>? BestRunTimes { get; set; }
    }
}
