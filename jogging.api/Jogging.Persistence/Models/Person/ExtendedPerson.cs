using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Jogging.Persistence.Models.Account;
using Jogging.Persistence.Models.Address;
using Jogging.Persistence.Models.Registration;
using Jogging.Persistence.Models.School;

namespace Jogging.Persistence.Models.Person;

public partial class ExtendedPerson : SimplePerson
{
    //public int Id { get; set; }

    //public string LastName { get; set; } = null!;

    //public string FirstName { get; set; } = null!;

    //public DateOnly BirthDate { get; set; }

    //public string? Ibannumber { get; set; }

    //public int? SchoolId { get; set; }

    //public int? AddressId { get; set; }

    //public Guid? UserId { get; set; }

    //public string? Gender { get; set; }

    //public string? Email { get; set; }

    //public int? RunningClubId { get; set; }

    public virtual Profile? Profile { get; set; }

    public virtual ExtendedAddress? Address { get; set; }

    public virtual ICollection<ExtendedRegistration> Registrations { get; set; } = new List<ExtendedRegistration>();

    public virtual Runningclub? RunningClub { get; set; }
    [JsonIgnore]
    public virtual ExtendedSchool? School { get; set; }
}
