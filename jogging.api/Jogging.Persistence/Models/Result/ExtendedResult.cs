using Jogging.Persistence.Models.CompetitionPerCategory;
using Jogging.Persistence.Models.Person;

namespace Jogging.Persistence.Models.Result;

public class ExtendedResult  : SimpleResult
{ 
    public CompetitionResultCompetitionPerCategory? CompetitionPerCategory { get; set; }
    public ExtendedPerson? Person { get; set; }
}