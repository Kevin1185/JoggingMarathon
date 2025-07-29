using Jogging.Persistence.Models.Person;

namespace Jogging.Persistence.Models;

public partial class Runningclub
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Url { get; set; }

    public bool AdminChecked { get; set; }

    public byte[]? Logo { get; set; }

    public virtual ICollection<ExtendedPerson> People { get; set; } = new List<ExtendedPerson>();
}
