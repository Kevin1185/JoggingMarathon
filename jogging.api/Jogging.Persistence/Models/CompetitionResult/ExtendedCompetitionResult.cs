using Jogging.Persistence.Models.CompetitionPerCategory;
using Jogging.Persistence.Models.Person;
using Microsoft.EntityFrameworkCore;

namespace Jogging.Persistence.Models.CompetitionResult;

public partial class ExtendedCompetitionResult : SimpleCompetitionResult
{
    public CompetitionResultCompetitionPerCategory? CompetitionPerCategory { get; set; }
    public AdvancedPerson? Person { get; set; }
}
