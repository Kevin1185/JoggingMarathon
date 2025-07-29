using Jogging.Persistence.Models.Person;
using System;
using System.Collections.Generic;

namespace Jogging.Persistence.Models.Account;

public partial class Profile
{
    public Guid ProfileId { get; set; } = Guid.NewGuid();

    public string? Role { get; set; }

    public ExtendedPerson Person { get; set; }
}
