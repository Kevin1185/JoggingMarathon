using Jogging.Persistence.Models.Competition;
using Jogging.Persistence.Models.Person;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jogging.Persistence.Models.Registration;

public partial class PersonRegistration
{
    public int Id { get; set; }
    public short? RunNumber { get; set; }
    public TimeSpan? RunTime { get; set; }
    public int CompetitionPerCategoryId { get; set; }
    public bool? Paid { get; set; }
    public int CompetitionId { get; set; }
    public int PersonId { get; set; }
    public ExtendedPerson? Person { get; set; }
    public ExtendedCompetition? Competition { get; set; }
}
