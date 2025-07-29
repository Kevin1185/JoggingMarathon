namespace Jogging.Persistence.Models;

public partial class Personview
{
    public int Id { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public DateOnly BirthDate { get; set; }

    public string? Ibannumber { get; set; }

    public int? SchoolId { get; set; }

    public int? AddressId { get; set; }

    public Guid? UserId { get; set; }

    public string? Gender { get; set; }

    public string? Email { get; set; }

    public string Fullname { get; set; } = null!;
}
