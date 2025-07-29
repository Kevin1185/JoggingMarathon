using System;
using System.Collections.Generic;
using Jogging.Persistence.Models.AgeCategory;
using Jogging.Persistence.Models.Competition;
using Jogging.Persistence.Models.Registration;

namespace Jogging.Persistence.Models.CompetitionPerCategory;

public partial class ExtendedCompetitionpercategory : SimpleCompetitionpercategory
{
    //public int Id { get; set; }

    //public string? Gender { get; set; }

    //public string? DistanceName { get; set; }

    //public float? DistanceInKm { get; set; }

    //public int? AgeCategoryId { get; set; }

    //public int? CompetitionId { get; set; }

    //public DateTime? GunTime { get; set; }

    public virtual SimpleAgeCategory? AgeCategory { get; set; }

    public virtual ExtendedCompetition? Competition { get; set; }

    public virtual ICollection<ExtendedRegistration> Registrations { get; set; } = new List<ExtendedRegistration>();
}
