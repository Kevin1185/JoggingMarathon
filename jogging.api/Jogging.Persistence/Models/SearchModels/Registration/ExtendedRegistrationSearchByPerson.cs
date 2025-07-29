namespace Jogging.Persistence.Models.SearchModels.Registration;
public class ExtendedRegistrationSearchByPerson
{
    // Registration fields
    public int RegistrationId { get; set; }
    public short? RunNumber { get; set; }    
    public TimeSpan? RunTime { get; set; }
    public int CompetitionPerCategoryId { get; set; }
    public bool? Paid { get; set; }
    public int PersonId { get; set; }
    public int CompetitionId { get; set; }
    
    // CompetitionPerCategory fields
    public string? DistanceName { get; set; }
    
    // Person fields
    public string? LastName { get; set; }
    public string? FirstName { get; set; }
    public DateOnly BirthDate { get; set; }
    public string? IbanNumber { get; set; }
    public int? SchoolId { get; set; }
    public int AddressId { get; set; }
    public Char Gender { get; set; }
    public string? UserId { get; set; }
    
    // Address fields
    public string? Street { get; set; }
    public string? HouseNumber { get; set; }
    public string? City { get; set; }
    public string? ZipCode { get; set; }
}