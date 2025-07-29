using Jogging.Persistence.Models.Competition;
using Jogging.Persistence.Models.CompetitionPerCategory;
using Jogging.Persistence.Models.Person;

namespace Jogging.Persistence.Models.Registration;

public partial class ExtendedRegistration : SimpleRegistration
{
    //public int Id { get; set; }

    //public short? RunNumber { get; set; }

    //public string? RunTime { get; set; }

    //public int? CompetitionPerCategoryId { get; set; }

    //public bool Paid { get; set; }

    //public int? PersonId { get; set; }

    //public int? CompetitionId { get; set; }

    //public CompetitionResultCompetitionPerCategory CompetitionPerCategory { get; set; }

    public virtual ExtendedCompetition? Competition { get; set; }

    public virtual ExtendedCompetitionpercategory? CompetitionPerCategory { get; set; }

    public virtual ExtendedPerson? Person { get; set; }
}
