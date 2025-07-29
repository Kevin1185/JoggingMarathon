using Jogging.Persistence.Models.Address;
using Jogging.Persistence.Models.School;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jogging.Persistence.Models.Person;
public partial class SimplePerson
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

    public int? RunningClubId { get; set; }
   
}
