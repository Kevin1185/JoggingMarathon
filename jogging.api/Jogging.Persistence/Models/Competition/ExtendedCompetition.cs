using Jogging.Persistence.Models.Address;
using Jogging.Persistence.Models.CompetitionPerCategory;
using Jogging.Persistence.Models.Registration;

namespace Jogging.Persistence.Models.Competition;

public partial class ExtendedCompetition : SimpleCompetition
{
    public virtual ExtendedAddress? Address { get; set; }
    public virtual ICollection<ExtendedCompetitionpercategory> Competitionpercategories { get; set; } = new List<ExtendedCompetitionpercategory>();
    public virtual ICollection<ExtendedRegistration> Registrations { get; set; } = [];
}
