using Jogging.Persistence.Models.Address;

namespace Jogging.Persistence.Models.Competition
{
    public partial class SimpleCompetition
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public DateTime? Date { get; set; }

        public int? AddressId { get; set; }

        public bool? Active { get; set; }

        public string? ImgUrl { get; set; }

        public string? Information { get; set; }

        public string? Url { get; set; }

        public bool? RankingActive { get; set; }

        public virtual SimpleAddress? Address { get; set; }
        
    }
}
