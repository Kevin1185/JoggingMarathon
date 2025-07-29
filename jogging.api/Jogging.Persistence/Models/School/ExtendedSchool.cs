using System.Text.Json.Serialization;
using Jogging.Persistence.Models.Person;

namespace Jogging.Persistence.Models.School;

public partial class ExtendedSchool : SimpleSchool
{
    [JsonIgnore]
    public virtual ICollection<ExtendedPerson> People { get; set; } = new List<ExtendedPerson>();
}
