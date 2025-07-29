namespace Jogging.Persistence.Models.Person;

public class ExtendedPersonSearch
{
    public int PersonId { get; set; }
    public string? LastName { get; set; }
    public string? FirstName { get; set; }
    public DateOnly BirthDate { get; set; }
    public string? IbanNumber { get; set; }    
    public int? SchoolId { get; set; }
    public int AddressId { get; set; }
    public Char Gender { get; set; }
    public string? Email { get; set; }
    public string? UserId { get; set; }
    
    // Address fields
    public string? Street { get; set; }
    public string? HouseNumber { get; set; }
    public string? City { get; set; }
    public string? ZipCode { get; set; }
    
    // Profile fields
    public string? Role { get; set; }
}