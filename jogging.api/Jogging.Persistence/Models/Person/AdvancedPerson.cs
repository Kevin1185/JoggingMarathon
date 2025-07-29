using Jogging.Persistence.Models.Address;

namespace Jogging.Persistence.Models.Person;

public partial class AdvancedPerson
{
    public int Id { get; set; }
    public string? LastName { get; set; }
    public string? FirstName { get; set; }
    public DateOnly BirthDate { get; set; }
    public string? IBANNumber { get; set; }
    public int? SchoolId { get; set; }
    public int? AddressId { get; set; }
    public string? UserId { get; set; }
    public string? Email { get; set; }
    public Char Gender { get; set; }
    public SimpleAddress? Address { get; set; }
}
