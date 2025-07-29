using System;
using System.Collections.Generic;
using Jogging.Persistence.Models.CompetitionPerCategory;

namespace Jogging.Persistence.Models.AgeCategory;

public partial class SimpleAgeCategory
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? MinimumAge { get; set; }

    public int? MaximumAge { get; set; }

    public virtual ICollection<ExtendedCompetitionpercategory> Competitionpercategories { get; set; } = new List<ExtendedCompetitionpercategory>();
}
