using System;
using System.Collections.Generic;
using Jogging.Persistence.Models.Competition;
using Jogging.Persistence.Models.Person;

namespace Jogging.Persistence.Models.Address;

public partial class ExtendedAddress : SimpleAddress
{
    //public int Id { get; set; }

    //public string? Street { get; set; }

    //public string City { get; set; } = null!;

    //public string? HouseNumber { get; set; }

    //public string? ZipCode { get; set; }

    public virtual ICollection<ExtendedCompetition> Competitions { get; set; } = new List<ExtendedCompetition>();

    public virtual ICollection<ExtendedPerson> People { get; set; } = new List<ExtendedPerson>();
}
